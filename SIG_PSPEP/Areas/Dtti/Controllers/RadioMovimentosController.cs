using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Areas.Dtti.Models;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SIG_PSPEP.Areas.Dtti.Controllers
{
    [Area("Dtti")]
    public class RadioMovimentosController : Controller
    {
        private readonly AppDbContext _context;

        public RadioMovimentosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Dtti/RadioMovimentos
        public async Task<IActionResult> Saida()
        {
            var appDbContext = _context.RadioMovimentos
                .Include(r => r.OrgUnidPnaMinint)
                .Include(r => r.Radio)
                    .ThenInclude(rt => rt.RadioTipo)
                .Include(r => r.RadioGuia)
                .Include(r => r.User)
                .Where(r => r.TipoMovimento == "Saída")
                .OrderByDescending(r => r.DataRegisto);

            return View(await appDbContext.ToListAsync());
        }

        // GET: Dtti/RadioMovimentos
        public async Task<IActionResult> Entrada()
        {
            var appDbContext = _context.RadioMovimentos
     .Include(r => r.OrgUnidPnaMinint)
     .Include(r => r.Radio)
         .ThenInclude(rt => rt.RadioTipo)
     .Include(r => r.RadioGuia)
     .Include(r => r.User)
     .Where(r => r.TipoMovimento == "Entrada")
     .OrderByDescending(r => r.DataRegisto); // 👈 ordena por data mais recente;

            return View(await appDbContext.ToListAsync());
        }


        // GET: Dtti/RadioMovimentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var radioMovimento = await _context.RadioMovimentos
                .Include(r => r.OrgUnidPnaMinint)
                .Include(r => r.Radio)
                .Include(r => r.RadioGuia)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (radioMovimento == null)
            {
                return NotFound();
            }

            return View(radioMovimento);
        }

        // GET: Dtti/RadioMovimentos/Create
        public IActionResult Create()
        {
            ViewData["OrgUnidPnaMinintId"] = new SelectList(_context.OrgUnidPnaMinints, "Id", "Sigla");
            ViewBag.Radios = (
     from r in _context.Radios
     where r.Estado == true && r.EstadoTecnico == "Funcional"
     let ultimo = _context.RadioMovimentos
                     .Where(m => m.RadioId == r.Id)
                     .OrderByDescending(m => m.DataRegisto)
                     .Select(m => m.TipoMovimento)
                     .FirstOrDefault()
     where ultimo == null || ultimo == "Entrada"
     select new
     {
         r.Id,
         r.CodRadio,
         Marca = r.RadioTipo.Marca,
         Modelo = r.RadioTipo.Modelo,
         r.IdRadio,
         r.NumSerie
     }
 ).ToList();

            return PartialView("_Create", new RadioMovimento());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( int OrgUnidPnaMinintId, string? observacao, List<int> radiosSelecionadas)
        {
            if (radiosSelecionadas == null || radiosSelecionadas.Count == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Selecione pelo menos uma rádio para movimentar."
                });
            }

            try
            {
                // 1️⃣ Criar a guia principal
                var guia = new RadioGuia
                {
                    NumGuia = GerarNumeroGuia(),
                    Observacao = observacao,
                    Aprovado = false,
                    DataRegisto = DateTime.Now,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };

                _context.RadioGuias.Add(guia);
                await _context.SaveChangesAsync();

                // 2️⃣ Criar os movimentos múltiplos
                foreach (var radioId in radiosSelecionadas)
                {
                    var movimento = new RadioMovimento
                    {
                        RadioId = radioId,
                        RadioGuiaId = guia.Id,
                        OrgUnidPnaMinintId = OrgUnidPnaMinintId,
                        TipoMovimento = "Saída",
                        DataRegisto = DateTime.Now,
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    };

                    _context.RadioMovimentos.Add(movimento);
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = $"Guia {guia.NumGuia} criada com {radiosSelecionadas.Count} movimentações."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Erro ao criar guia: " + ex.Message
                });
            }
        }


        // GET: Dtti/RadioMovimentos/Create
        public IActionResult CreateOut()
        {
            ViewBag.Radios = (
                from r in _context.Radios
                where r.Estado == true && r.EstadoTecnico == "Funcional"
                let ultimoMovimento = _context.RadioMovimentos
                    .Where(m => m.RadioId == r.Id)
                    .OrderByDescending(m => m.DataRegisto)
                    .FirstOrDefault()
                where ultimoMovimento != null
                      && ultimoMovimento.TipoMovimento == "Saída"
                      && ultimoMovimento.RadioGuia.Aprovado == true
                select new
                {
                    r.Id,
                    r.CodRadio,
                    Marca = r.RadioTipo.Marca,
                    Modelo = r.RadioTipo.Modelo,
                    r.IdRadio,
                    r.NumSerie
                }
            ).ToList();

            return PartialView("_CreateOut", new RadioMovimento());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOut(int orgaoUnidadeId, string? observacao, List<int> radiosSelecionadas)
        {
            if (radiosSelecionadas == null || radiosSelecionadas.Count == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Selecione pelo menos uma rádio para movimentar."
                });
            }

            try
            {
                int contador = 0;

                foreach (var radioId in radiosSelecionadas)
                {

                    var ultimoMovimento = await _context.RadioMovimentos
                        .Where(m => m.RadioId == radioId)
                        .OrderByDescending(m => m.DataRegisto)
                        .FirstOrDefaultAsync();

                    if (ultimoMovimento == null)
                        continue;

                    if (!string.Equals(ultimoMovimento.TipoMovimento, "Saída", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var movimento = new RadioMovimento
                    {
                        RadioId = radioId,
                        RadioGuiaId = ultimoMovimento.RadioGuiaId, // reutiliza a guia
                        OrgUnidPnaMinintId = 22, //ID do Departamento do DTTI
                        TipoMovimento = "Entrada",
                        DataRegisto = DateTime.Now,
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    };

                    _context.RadioMovimentos.Add(movimento);
                    contador++;
                }

                if (contador == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Nenhuma rádio elegível para devolução (somente rádios com último movimento 'Saída')."
                    });
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = $"Devolvido {contador} rádio(s) para o DTTI com sucesso."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Erro ao criar movimento de devolução: " + ex.Message
                });
            }
        }
        private string GerarNumeroGuia()
        {
            var ano = DateTime.Now.Year;
            var sequencia = (_context.RadioGuias.Count() + 1).ToString("D4"); // 0001, 0002, ...
            return $"{sequencia}/{ano}/DTTI/PSPEP/PNA";
        }

        public async Task<IActionResult> Localizacao(
     int? tipoId,
     int? orgaoUnidadeId,
     string estadoTecnico,
     string marca,
     string modelo)
        {
            // 🔹 Carregar todas as rádios e seus tipos
            var radios = await _context.Radios
                .Include(r => r.RadioTipo)
                .ToListAsync();

            // 🔹 Carregar movimentos (para localizar cada rádio)
            var movimentos = await _context.RadioMovimentos
                .Include(r => r.OrgUnidPnaMinint)
                .ToListAsync();

            // 🔹 Montar a lista final (último movimento de cada rádio)
            var radiosComLocalizacao = radios
                .Select(radio =>
                {
                    var ultimoMov = movimentos
                        .Where(m => m.RadioId == radio.Id)
                        .OrderByDescending(m => m.DataRegisto)
                        .FirstOrDefault();

                    return new RadioMovimento
                    {
                        Radio = radio,
                        OrgUnidPnaMinint = ultimoMov?.OrgUnidPnaMinint ?? new OrgUnidPnaMinint { Sigla = "DTTI/PSPEP" },
                        DataRegisto = ultimoMov?.DataRegisto ?? DateTime.MinValue
                    };
                })
                .AsQueryable();

            // 🔹 Aplicar filtros
            if (tipoId.HasValue)
                radiosComLocalizacao = radiosComLocalizacao
                    .Where(r => r.Radio.RadioTipoId == tipoId.Value);

            if (orgaoUnidadeId.HasValue)
                radiosComLocalizacao = radiosComLocalizacao
                    .Where(r => r.OrgUnidPnaMinint.Id == orgaoUnidadeId.Value);

            if (!string.IsNullOrEmpty(estadoTecnico))
                radiosComLocalizacao = radiosComLocalizacao
                    .Where(r => r.Radio.EstadoTecnico == estadoTecnico);

            if (!string.IsNullOrEmpty(marca))
                radiosComLocalizacao = radiosComLocalizacao
                    .Where(r => r.Radio.RadioTipo.Marca.Contains(marca));

            if (!string.IsNullOrEmpty(modelo))
                radiosComLocalizacao = radiosComLocalizacao
                    .Where(r => r.Radio.RadioTipo.Modelo.Contains(modelo));

            // 🔹 Montar o ViewModel
            var viewModel = new RadioLocalizacaoViewModel
            {
                TipoId = tipoId,
                OrgaoUnidadeId = orgaoUnidadeId,
                EstadoTecnico = estadoTecnico,
                Marca = marca,
                Modelo = modelo,
                Tipos = await _context.RadioTipos.ToListAsync(),
                OrgaosMinint = await _context.OrgUnidPnaMinints.ToListAsync(),
                EstadosTecnicos = await _context.Radios
                    .Select(r => r.EstadoTecnico)
                    .Distinct()
                    .ToListAsync(),
                Radios = radiosComLocalizacao
                    .OrderByDescending(r => r.DataRegisto)
                    .ToList()
            };

            return View(viewModel);
        }

        // Página geral de histórico (lista de rádios)
        public async Task<IActionResult> Historico()
        {
            var radios = await _context.Radios
                .Include(r => r.RadioTipo)
                .Include(r => r.User)
                .ToListAsync();

            return View(radios);
        }

        public async Task<IActionResult> HistoricoRadios(int id)
        {
            var historico = await _context.RadioMovimentos
                .Include(r => r.OrgUnidPnaMinint)
                .Include(r => r.Radio)
                .Include(r => r.RadioGuia)
                .Include(r => r.User)
                .Where(r => r.RadioId == id)
                .OrderBy(r => r.DataRegisto)
                .ToListAsync();

            if (historico == null || !historico.Any())
                return PartialView("_HistoricoVazio");

            // Calcular dias que a rádio ficou em cada unidade
            var listaAnalitica = new List<object>();

            for (int i = 0; i < historico.Count; i++)
            {
                var atual = historico[i];
                var proximo = i < historico.Count - 1 ? historico[i + 1] : null;

                // Calcula diferença de dias até o próximo movimento, ou até hoje se for o último
                var dataFim = proximo?.DataRegisto ?? DateTime.Now;
                var dias = (dataFim - atual.DataRegisto).Days;

                listaAnalitica.Add(new
                {
                    Unidade = atual.OrgUnidPnaMinint?.Sigla ?? "—",
                    TipoMovimento = atual.TipoMovimento ?? "—",
                    DataInicio = atual.DataRegisto.ToString("dd/MM/yyyy"),
                    DataFim = dataFim.ToString("dd/MM/yyyy"),
                    Dias = dias < 0 ? 0 : dias
                });
            }

            ViewBag.Analise = listaAnalitica;

            return PartialView("_HistoricoRadios", historico);
        }


    }
}
