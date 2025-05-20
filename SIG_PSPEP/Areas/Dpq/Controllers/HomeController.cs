using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Areas.Dpq.Models;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Dpq.Controllers;

[Area("Dpq")]
//[Authorize(Roles = "Require_Admin_ChDepar_ChSec_Esp")]
public class HomeController : BaseController
{
    private readonly ILogger<EfectividadesController> _logger;
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;

    public HomeController(
        AppDbContext context,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ILogger<EfectividadesController> logger)
        : base(context)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        _logger = logger;
    }

    public IActionResult Index()
    {
        #region Segurança da Área
        if (!UsuarioTemAcessoArea("DPQ") && !UsuarioTemAcessoArea("ADMIN"))
        {
            return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
        }
        #endregion

        return View();
    }

    [HttpGet]
    public IActionResult DashboardData()
    {
        #region Segurança da Área
        if (!UsuarioTemAcessoArea("DPQ") && !UsuarioTemAcessoArea("ADMIN"))
        {
            return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
        }
        #endregion
        const int universoTotal = 110000;
        var hoje = DateTime.Today;

        var efectivos = _context.Efectivos
            .Include(e => e.Patente)
            .Include(e => e.OrgaoUnidade)
            .ToList();

        int totalEfectivos = efectivos.Count;
        double percTotal = Math.Round((double)totalEfectivos / universoTotal * 100, 2);

        // Por Género
        int totalMasculino = efectivos.Count(e => e.Genero?.ToLower() == "masculino");
        int totalFeminino = efectivos.Count(e => e.Genero?.ToLower() == "feminino");

        double percMasculino = totalEfectivos > 0 ? Math.Round((double)totalMasculino / totalEfectivos * 100, 2) : 0;
        double percFeminino = totalEfectivos > 0 ? Math.Round((double)totalFeminino / totalEfectivos * 100, 2) : 0;

        // Por Classe de Patente
        var classes = new[] { "Oficial Comissário", "Oficial Superior", "Oficial Subalterno", "Subchefe", "Agente" };
        var porClasse = classes
            .Select(classe => new
            {
                Classe = classe,
                Total = efectivos.Count(e => e.Patente?.Classe == classe)
            })
            .Select(x => new
            {
                x.Classe,
                x.Total,
                Percentagem = totalEfectivos > 0 ? Math.Round((double)x.Total / totalEfectivos * 100, 2) : 0
            }).ToList();

        // Por Unidade
        var siglasIndividuais = new[] { "UPP", "UPD", "UPJ", "UPEP", "UGHH", "UBM", "GABCMDTE", "GAB2ºCMDTE", "GABCEM" };

        // Primeiro agrupa como já fazes
        var agrupados = efectivos
            .GroupBy(e =>
            {
                var sigla = e.OrgaoUnidade?.Sigla?.ToUpper();
                return siglasIndividuais.Contains(sigla) ? sigla : "ORGÃOS";
            })
            .Select(g => new
            {
                Unidade = g.Key,
                Total = g.Count(),
                Percentagem = totalEfectivos > 0 ? Math.Round((double)g.Count() / totalEfectivos * 100, 2) : 0
            })
            .ToList();

        // Agora cria a soma dos três gabinetes
        var totalGabinetes = agrupados
            .Where(g => g.Unidade == "GABCMDTE" || g.Unidade == "GAB2ºCMDTE" || g.Unidade == "GABCEM")
            .ToList();

        var gabinetesItem = new
        {
            Unidade = "GABINETES",
            Total = totalGabinetes.Sum(g => g.Total),
            Percentagem = totalGabinetes.Sum(g => g.Percentagem) // ou faz média se quiser: totalGabinetes.Average(g => g.Percentagem)
        };

        // Junta o item "GABINETES" com os demais
        var porUnidade = agrupados
            .Where(g => g.Unidade != "GABCMDTE" && g.Unidade != "GAB2ºCMDTE" && g.Unidade != "GABCEM")
            .Append(gabinetesItem)
            .ToList();

        var faixas = new[]
        {
        new { Faixa = "18-25", Min = 18, Max = 25 },
        new { Faixa = "26-30", Min = 26, Max = 30 },
        new { Faixa = "31-35", Min = 31, Max = 35 },
        new { Faixa = "36-40", Min = 36, Max = 40 },
        new { Faixa = "41-50", Min = 41, Max = 50 },
        new { Faixa = "51-55", Min = 51, Max = 55 },
        new { Faixa = "56-60", Min = 56, Max = 60 },
        new { Faixa = "+60",   Min = 61, Max = int.MaxValue }
    };

        var porFaixaEtaria = faixas.Select(faixa =>
        {
            var total = efectivos.Count(e =>
            {
                if (e.DataNasc == default) return false;
                int idade = hoje.Year - e.DataNasc.Year;
                if (e.DataNasc.Date > hoje.AddYears(-idade)) idade--;
                return idade >= faixa.Min && idade <= faixa.Max;
            });

            return new
            {
                faixa.Faixa,
                Total = total,
                Percentagem = totalEfectivos > 0 ? Math.Round((double)total / totalEfectivos * 100, 2) : 0
            };
        }).ToList();

        // Resultado Final
        var resultado = new
        {
            TotalEfectivos = totalEfectivos,
            PercentagemDoUniverso = percTotal,
            PorGenero = new[]
            {
            new { Genero = "Masculino", Total = totalMasculino, Percentagem = percMasculino },
            new { Genero = "Feminino", Total = totalFeminino, Percentagem = percFeminino }
        },
            PorClasse = porClasse,
            PorUnidade = porUnidade,
            PorFaixaEtaria = porFaixaEtaria
        };

        return Json(resultado);
    }

    [HttpGet]
    public async Task<IActionResult> GraficoEfetivosPorUnidadeMes()
    {
        #region Segurança da Área
        if (!UsuarioTemAcessoArea("DPQ") && !UsuarioTemAcessoArea("ADMIN"))
        {
            return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
        }
        #endregion
        var hoje = DateTime.Today;
        var inicio = new DateTime(hoje.Year, hoje.Month, 1).AddMonths(-11); // primeiro dia do mês, 11 meses atrás

        // Consulta base
        var consulta = await _context.Efectivos
            .Where(e => e.DataRegisto >= inicio)
            .Include(e => e.OrgaoUnidade)
            .GroupBy(e => new
            {
                Unidade = e.OrgaoUnidade.Sigla,
                MesAno = new DateTime(e.DataRegisto.Year, e.DataRegisto.Month, 1)
            })
            .Select(g => new
            {
                Unidade = g.Key.Unidade ?? "Indefinido",
                Mes = g.Key.MesAno.ToString("yyyy-MM"),
                Total = g.Count()
            })
            .ToListAsync();

        // Lista dos últimos 12 meses (formato yyyy-MM)
        var meses = Enumerable.Range(0, 12)
            .Select(i => hoje.AddMonths(-11 + i).ToString("yyyy-MM"))
            .ToList();

        // Lista de unidades únicas
        var unidades = consulta
            .Select(d => d.Unidade)
            .Distinct()
            .OrderBy(u => u)
            .ToList();

        // Preparar dados para o gráfico
        var datasets = unidades.Select(unidade => new
        {
            label = unidade,
            data = meses.Select(mes =>
                consulta.FirstOrDefault(d => d.Unidade == unidade && d.Mes == mes)?.Total ?? 0
            ).ToList()
        });

        return Json(new
        {
            labels = meses,
            datasets = datasets
        });
    }

    public async Task<IActionResult> QuantidadeEfectivosPorPatente()
    {
        #region Segurança da Área
        if (!UsuarioTemAcessoArea("DPQ") && !UsuarioTemAcessoArea("ADMIN"))
        {
            return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
        }
        #endregion
        var efectivos = await _context.Efectivos
            .Include(e => e.Patente)
            .Include(e => e.OrgaoUnidade)
            .ToListAsync();

        // Total por órgão
        var totaisPorOrgao = efectivos
            .GroupBy(e => e.OrgaoUnidade?.Sigla ?? "Sem Órgão")
            .ToDictionary(g => g.Key, g => g.Count());

        // Agrupar por Patente e Orgao
        var agrupado = efectivos
            .GroupBy(e => new
            {
                Patente = e.Patente?.Posto ?? "Sem Patente",
                Orgao = e.OrgaoUnidade?.Sigla ?? "Sem Órgão"
            })
            .Select(g => new
            {
                g.Key.Patente,
                g.Key.Orgao,
                Quantidade = g.Count()
            })
            .ToList();

        // Gerar estrutura final
        var resultado = agrupado
            .GroupBy(x => x.Patente)
            .Select(g => new GraficoEfectivosPorPatenteDTO
            {
                Patente = g.Key,
                QuantidadePorOrgaoUnidade = g.ToDictionary(x => x.Orgao, x => x.Quantidade),
                PercentagemPorOrgaoUnidade = g.ToDictionary(x => x.Orgao,
                    x => Math.Round((double)x.Quantidade * 100 / totaisPorOrgao[x.Orgao], 2))
            })
            .ToList();

        return Json(resultado);
    }

}
