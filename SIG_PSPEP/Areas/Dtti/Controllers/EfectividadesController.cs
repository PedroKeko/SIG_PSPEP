using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Entidades;
using SIG_PSPEP.Context;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIG_PSPEP.Areas.Dpq.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace SIG_PSPEP.Areas.Dtti.Controllers
{
    [Area("Dtti")]
    [Authorize(Policy = "Require_Admin_ChDepar_ChSec_Esp")]
    public class EfectividadesController : BaseController
    {
        private readonly ILogger<EfectividadesController> _logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public EfectividadesController(
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

        public async Task<IActionResult> SelecionarParaLancamentoLocal(DateTime? data)
        {
            if (!UsuarioTemAcessoArea("DTTI"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            var efetivos = await _context.Efectivos
                .Include(e => e.OrgaoUnidade).Where(e => e.OrgaoUnidade != null && e.OrgaoUnidade.Sigla.ToUpper() == "DTTI")
                .Select(e => new EfectivoSelecao
                {
                    Id = e.Id,
                    Patente = e.Patente.Posto,
                    NomeCompleto = e.NomeCompleto,
                    NIP = e.NIP,
                    OrgaoUnidade = e.OrgaoUnidade!.Sigla,
                    Selecionado = false
                }).ToListAsync();

            var viewModel = new EfectivoFiltroOrgaoUnidadeModel
            {
                Efectivos = efetivos,
                DataSelecionada = data
            };

            return PartialView("_SelecionarParaLancamento", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SalvarParaLancamento([FromBody] EfectivoFiltroOrgaoUnidadeModel model)
        {
            if (!UsuarioTemAcessoArea("DTTI"))
            {
                return Forbid();
            }

            if (model.DataSelecionada == null)
            {
                return Json(new { sucesso = false, mensagem = "Por favor, selecione uma data." });
            }

            var mensagensErro = new List<string>();
            var data = model.DataSelecionada.Value.Date;

            foreach (var item in model.Efectivos)
            {
                // Verifica se já existe lançamento para a data
                var jaExiste = await _context.EfectivoEfectividades
                    .AnyAsync(x => x.EfectivoId == item.Id && x.DataPresenca == data);

                if (jaExiste)
                {
                    mensagensErro.Add($"O efectivo {item.NomeCompleto} já está lançado para o dia {data:dd/MM/yyyy}.");
                    continue;
                }

                // Checkbox TRUE → Ausente (2)
                // Checkbox FALSE → Presente (1)
                var efectividadeTipo = item.Selecionado ? 2 : 1;

                var novaPresenca = new EfectivoEfectividade
                {
                    EfectivoId = item.Id,
                    DataPresenca = data,
                    EfectividadeTipoId = efectividadeTipo
                };

                _context.EfectivoEfectividades.Add(novaPresenca);
            }

            await _context.SaveChangesAsync();

            if (mensagensErro.Any())
            {
                return Json(new { sucesso = false, mensagem = "Alguns lançamentos falharam.", erros = mensagensErro });
            }

            return Json(new { sucesso = true, mensagem = "Lançamentos efetuados com sucesso!" });
        }

        // GET: LancamentoIndividual
        public async Task<IActionResult> LancamentoIndividual()
        {
            if (!UsuarioTemAcessoArea("DTTI"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            ViewBag.Efectivos = await _context.Efectivos
                 .Include(e => e.OrgaoUnidade).Where(e => e.OrgaoUnidade != null && e.OrgaoUnidade.Sigla.ToUpper() == "DTTI")
                .OrderBy(e => e.NomeCompleto)
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.NomeCompleto
                })
                .ToListAsync();

            ViewBag.EfectividadeTipos = await _context.EfectividadeTipos
                .OrderBy(t => t.DescTipo)
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.DescTipo
                })
                .ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LancamentoIndividual(EfectivoEfectividade efetividade)
        {
            if (!UsuarioTemAcessoArea("DTTI"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            if (ModelState.IsValid)
            {
                efetividade.DataRegisto = DateTime.Now;
                efetividade.Estado = true;

                _context.EfectivoEfectividades.Add(efetividade);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Efetividade lançada com sucesso.";
                return RedirectToAction(nameof(LancamentoIndividual));
            }

            ViewBag.Efectivos = await _context.Efectivos.ToListAsync();
            ViewBag.EfectividadeTipos = await _context.EfectividadeTipos.ToListAsync();
            return View(efetividade);
        }


        public async Task<IActionResult> LancamentoLocal()
        {
            if (!UsuarioTemAcessoArea("DTTI"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }

            ViewBag.Efectivos = await _context.Efectivos
                .Include(e => e.OrgaoUnidade)
                .Where(e => e.OrgaoUnidade != null && e.OrgaoUnidade.Sigla.ToUpper() == "DTTI")
                .ToListAsync();
            ViewBag.EfectividadeTipos = await _context.EfectividadeTipos.ToListAsync();

            var model = new CalendarPageModel
            {
                CurrentMonth = DateTime.Today.Month,
                CurrentYear = DateTime.Today.Year
            };

            // Define o intervalo: mês anterior, mês atual e próximo mês
            var primeiroDiaMesAtual = new DateTime(model.CurrentYear, model.CurrentMonth, 1);
            var primeiroDiaMesAnterior = primeiroDiaMesAtual.AddMonths(-1);
            var ultimoDiaProximoMes = primeiroDiaMesAtual.AddMonths(2).AddDays(-1);

            // Busca todos os dados no período
            var dadosDoPeriodo = _context.EfectivoEfectividades
                .Include(e => e.Efectivo)
                .ThenInclude(e => e.OrgaoUnidade)
                .Where(e => e.Efectivo != null && e.Efectivo.OrgaoUnidade != null && e.Efectivo.OrgaoUnidade.Sigla.ToUpper() == "DPQ")
                .Where(e => e.DataPresenca.HasValue &&
                            e.DataPresenca.Value.Date >= primeiroDiaMesAnterior &&
                            e.DataPresenca.Value.Date <= ultimoDiaProximoMes)
                .ToList();

            // Agrupa por dia e calcula percentuais
            model.Resumos = dadosDoPeriodo
                .GroupBy(e => e.DataPresenca!.Value.Date)
                .Select(g =>
                {
                    var total = g.Count();
                    var presente = g.Count(x => x.EfectividadeTipoId == 1);
                    var ausente = g.Count(x => x.EfectividadeTipoId == 2);
                    var dispensado = g.Count(x => x.EfectividadeTipoId == 3);
                    var justificado = g.Count(x => x.EfectividadeTipoId == 4);

                    return new ResumoDiaPercentual
                    {
                        Data = g.Key.ToString("yyyy-MM-dd"),
                        Presente = presente,
                        Ausente = ausente,
                        Dispensado = dispensado,
                        Justificado = justificado,
                        PresentePct = total > 0 ? (presente * 100.0 / total) : 0,
                        AusentePct = total > 0 ? (ausente * 100.0 / total) : 0,
                        DispensadoPct = total > 0 ? (dispensado * 100.0 / total) : 0,
                        JustificadoPct = total > 0 ? (justificado * 100.0 / total) : 0
                    };
                })
                .ToList();

            return View(model);
        }

        // GET: LancamentoMultiplo
        public async Task<IActionResult> LancIndividual()
        {
            if (!UsuarioTemAcessoArea("DTTI"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            return View();
        }

        public async Task<IActionResult> ConsultarEfectividadeLocal(int? month, int? year)
        {
            if (!UsuarioTemAcessoArea("DTTI"))
            {
                return Forbid();
            }

            // Usar mês/ano atual caso não informado
            int mes = month ?? DateTime.Today.Month;
            int ano = year ?? DateTime.Today.Year;

            // Ajusta ano se mês for inválido
            if (mes < 1) { mes = 12; ano--; }
            if (mes > 12) { mes = 1; ano++; }

            // 1️⃣ Buscar todos os efectivos da unidade DPQ
            var efectivosDPQ = await _context.Efectivos
                .Include(e => e.OrgaoUnidade)
                .Include(e => e.Patente)
                .Where(e => e.OrgaoUnidade != null && e.OrgaoUnidade.Sigla == "DTTI")
                .ToListAsync();

            // 2️⃣ Buscar todas as efetividades do mês
            var efectividadesDoMes = await _context.EfectivoEfectividades
                .Include(e => e.Efectivo)
                .Where(e => e.DataPresenca.HasValue
                            && e.DataPresenca.Value.Month == mes
                            && e.DataPresenca.Value.Year == ano
                            && e.Efectivo.OrgaoUnidade != null
                            && e.Efectivo.OrgaoUnidade.Sigla == "DTTI")
                .ToListAsync();

            // 3️⃣ Criar lista final
            var listaMensal = new List<EfectivoDiaStatus>();
            int diasNoMes = DateTime.DaysInMonth(ano, mes);

            foreach (var efectivo in efectivosDPQ)
            {
                var status = new EfectivoDiaStatus
                {
                    EfectivoId = efectivo.Id,
                    NomeCompleto = efectivo.NomeCompleto ?? "",
                    Patente = efectivo.Patente?.Posto ?? "",
                    NIP = efectivo.NIP ?? "",
                    StatusPorDia = new Dictionary<int, string>()
                };

                // Inicializa estatísticas
                int presente = 0, ausente = 0, dispensado = 0, justificado = 0;

                for (int dia = 1; dia <= diasNoMes; dia++)
                {
                    var efetividade = efectividadesDoMes
                        .FirstOrDefault(e => e.EfectivoId == efectivo.Id && e.DataPresenca.Value.Day == dia);

                    string diaStatus = "";

                    if (efetividade != null)
                    {
                        switch (efetividade.EfectividadeTipoId)
                        {
                            case 1: diaStatus = "Presente"; presente++; break;
                            case 2: diaStatus = "Ausente"; ausente++; break;
                            case 3: diaStatus = "Dispensado"; dispensado++; break;
                            case 4: diaStatus = "Justificado"; justificado++; break;
                        }
                    }

                    status.StatusPorDia[dia] = diaStatus;
                }

                // Armazena estatísticas no objeto
                status.QuantidadePresente = presente;
                status.QuantidadeAusente = ausente;
                status.QuantidadeDispensado = dispensado;
                status.QuantidadeJustificado = justificado;

                listaMensal.Add(status);
            }

            // 4️⃣ Monta ViewModel
            var model = new CalendarMonthViewModel
            {
                Year = ano,
                Month = mes,
                Efectivos = listaMensal
            };

            return View(model);
        }

    }
}
