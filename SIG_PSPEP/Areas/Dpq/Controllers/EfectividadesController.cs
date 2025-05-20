using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Entidades;
using SIG_PSPEP.Context;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIG_PSPEP.Areas.Dpq.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace SIG_PSPEP.Areas.Dpq.Controllers
{
    [Area("Dpq")]
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

        [HttpGet]
        public async Task<IActionResult> SelecionarParaLancamento(DateTime? data)
        {
            if (!UsuarioTemAcessoArea("DPQ") && !UsuarioTemAcessoArea("ADMIN"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            var efetivos = await _context.Efectivos
                .Include(e => e.OrgaoUnidade)
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
        public async Task<IActionResult> SelecionarParaLancamento([FromBody] EfectivoFiltroOrgaoUnidadeModel model)
        {
            if (!UsuarioTemAcessoArea("DPQ") && !UsuarioTemAcessoArea("ADMIN"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            if (model.DataSelecionada == null)
            {
                return Json(new { sucesso = false, mensagem = "Por favor, selecione uma data." });
            }

            var efectividadeTipoPadrao = 1; // Tipo padrão ou pegue conforme tua lógica
            var mensagensErro = new List<string>();

            foreach (var item in model.Efectivos.Where(e => e.Selecionado))
            {
                var jaExiste = await _context.EfectivoEfectividades
                    .AnyAsync(x => x.EfectivoId == item.Id && x.DataPresenca == model.DataSelecionada);

                if (jaExiste)
                {
                    mensagensErro.Add($"O efectivo {item.NomeCompleto} já está lançado para a data {model.DataSelecionada:dd/MM/yyyy}.");
                    continue;
                }

                var novaPresenca = new EfectivoEfectividade
                {
                    EfectivoId = item.Id,
                    DataPresenca = model.DataSelecionada,
                    EfectividadeTipoId = efectividadeTipoPadrao
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
            if (!UsuarioTemAcessoArea("DPQ") && !UsuarioTemAcessoArea("ADMIN"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            ViewBag.Efectivos = await _context.Efectivos
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
            if (!UsuarioTemAcessoArea("DPQ") && !UsuarioTemAcessoArea("ADMIN"))
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

        // GET: LancamentoMultiplo
        public async Task<IActionResult> LancamentoMultiplo()
        {
            if (!UsuarioTemAcessoArea("DPQ") && !UsuarioTemAcessoArea("ADMIN"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            ViewBag.Efectivos = await _context.Efectivos.ToListAsync();
            ViewBag.EfectividadeTipos = await _context.EfectividadeTipos.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LancamentoMultiplo([FromBody] List<EfetividadeLancamentoViewModel> efetividades)
        {
            if (!UsuarioTemAcessoArea("DPQ") && !UsuarioTemAcessoArea("ADMIN"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            if (efetividades != null && efetividades.Any())
            {
                foreach (var item in efetividades)
                {
                    var entidade = new EfectivoEfectividade
                    {
                        EfectivoId = item.EfectivoId,
                        EfectividadeTipoId = item.EfectividadeTipoId,
                        DataPresenca = item.DataPresenca,
                        DataRegisto = DateTime.Now,
                        Estado = true
                    };

                    _context.EfectivoEfectividades.Add(entidade);
                }

                await _context.SaveChangesAsync();
                return Ok(new { mensagem = "Efetividades lançadas com sucesso." });
            }

            return BadRequest("Lista inválida.");
        }

        // GET: LancamentoMultiplo
        public async Task<IActionResult> LancIndividual()
        {
            if (!UsuarioTemAcessoArea("DPQ") && !UsuarioTemAcessoArea("ADMIN"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            return View();
        }

        // GET: Efectivos
        public async Task<IActionResult> ConsultarEfectivos()
        {
            if (!UsuarioTemAcessoArea("DPQ") && !UsuarioTemAcessoArea("ADMIN"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            var appDbContext = _context.Efectivos
                .Include(e => e.FuncaoCargo)
                .Include(e => e.Municipio)
                .Include(e => e.OrgaoUnidade)
                .Include(e => e.Patente)
                .Include(e => e.ProvinciaNascimento)
                .Include(e => e.ProvinciaResidencia)
                .Include(e => e.SituacaoEfectivo)
                .Include(e => e.User)
                .OrderByDescending(e => e.Patente.grau);
            return View(await appDbContext.ToListAsync());
        }

    }
}
