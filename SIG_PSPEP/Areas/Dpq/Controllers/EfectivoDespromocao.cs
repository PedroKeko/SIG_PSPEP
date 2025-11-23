using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Areas.Dpq.Models;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Dpq.Controllers
{
    [Area("Dpq")]
    [Authorize(Policy = "Require_Admin_ChDepar_ChSec_Esp")]
    public class EfectivoDespromocao(
    AppDbContext context,
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    ILogger<EfectivoDespromocao> logger) : BaseController(context)
    {
        private readonly ILogger<EfectivoDespromocao> _logger = logger;
        private readonly UserManager<IdentityUser> userManager = userManager;
        private readonly SignInManager<IdentityUser> signInManager = signInManager;

        // GET: Dpq/EfectivoOrdemServicos

        public async Task<IActionResult> Index()
        {
            if (!UsuarioTemAcessoArea("DPQ"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            var query = _context.EfectivoOrdemServicos
                .Include(e => e.Efectivo)
                .Include(e => e.Patente)
                .Include(e => e.OrdemServico)
                .Where(e => e.TipoPromocao == "d")
                .Include(e => e.User);

            var lista = await query.ToListAsync();
            return View(lista);
        }

        [HttpGet]
        public async Task<IActionResult> FiltrarTabela(DateTime? dataInicio, DateTime? dataFim)
        {
            if (!UsuarioTemAcessoArea("DPQ"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            var query = _context.EfectivoOrdemServicos
                .Include(e => e.Efectivo).Include(e => e.Patente)
                .Include(e => e.OrdemServico).Include(e => e.User)
                .Where(e => e.TipoPromocao == "d")
                .AsQueryable();

            if (dataInicio.HasValue && dataFim.HasValue)
            {
                var inicio = dataInicio.Value.Date;
                var fim = dataFim.Value.Date.AddDays(1).AddTicks(-1); // garante até 23:59:59

                query = query.Where(e => e.DataRegisto >= inicio && e.DataRegisto <= fim);
            }

            var resultado = await query.ToListAsync();
            return PartialView("_TabelaEfectivoOrdemServicos", resultado);
        }


        // GET: Dpq/EfectivoOrdemServicoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!UsuarioTemAcessoArea("DPQ"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            if (id == null)
            {
                return NotFound();
            }

            var efectivoOrdemServico = await _context.EfectivoOrdemServicos
                .Include(e => e.Efectivo)
                .Include(e => e.Patente)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (efectivoOrdemServico == null)
            {
                return NotFound();
            }

            return View(efectivoOrdemServico);
        }

        // GET: Dpq/EfectivoOrdemServicoes/Create
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec")]
        public IActionResult Create()
        {
            if (!UsuarioTemAcessoArea("DPQ"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            ViewData["OrdemServicoId"] = new SelectList(_context.OrdemServicos, "Id", "NumOrdemServico");
            ViewData["EfectivoId"] = new SelectList(_context.Efectivos, "Id", "NomeCompleto");
            ViewData["PatenteId"] = new SelectList(_context.Patentes, "Id", "Posto");
            return PartialView("_Create");
        }

        //Efectividade Singula
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Require_Admin_ChDepar")]
        public async Task<IActionResult> Create(EfectivoOrdemServico efectivoOrdemServico)
        {
            if (!UsuarioTemAcessoArea("DPQ"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var efectivo = await _context.Efectivos
                        .Include(e => e.Patente)
                        .FirstOrDefaultAsync(e => e.Id == efectivoOrdemServico.EfectivoId);

                    var novaPatente = await _context.Patentes.FindAsync(efectivoOrdemServico.PatenteId);

                    if (efectivo == null || novaPatente == null)
                        return Json(new { success = false, message = "Dados do efectivo ou da nova patente não encontrados." });

                    // ✅ Verifica duplicidade para a mesma Ordem de Serviço
                    bool existePromocao = await _context.EfectivoOrdemServicos
                        .AnyAsync(e => e.EfectivoId == efectivoOrdemServico.EfectivoId &&
                                       e.OrdemServicoId == efectivoOrdemServico.OrdemServicoId);

                    if (existePromocao)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Não pode ser despromovido nesta Ordem de Serviço."
                        });
                    }

                    if (efectivo.Patente != null && efectivo.Patente.grau <= novaPatente.grau)
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"A patente selecionada de {novaPatente.Posto} tem grau Superior ou igual à ({efectivo.Patente.Posto})."
                        });
                    }
                    efectivoOrdemServico.TipoPromocao = "d";
                    _context.Add(efectivoOrdemServico);
                    await _context.SaveChangesAsync();

                    efectivo.PatenteId = efectivoOrdemServico.PatenteId;
                    _context.Update(efectivo);

                    var ordemServico = await _context.OrdemServicos.FindAsync(efectivoOrdemServico.OrdemServicoId);

                    _context.EfectivoHistoricos.Add(new EfectivoHistorico
                    {
                        EfectivoId = efectivo.Id,
                        Tipo = "Despromoção",
                        Descricao = $"Despromoção para a patente de {novaPatente?.Posto} via Ordem de Serviço Nº {ordemServico?.NumOrdemServico}",
                        Data = DateTime.Now,
                        Status = "Aprovado"
                    });

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Json(new
                    {
                        success = true,
                        message = $"Despromoção para {novaPatente.Posto} realizada com sucesso."
                    });
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return Json(new { success = false, message = "Erro interno ao processar a despromoção." });
                }
            }

            return Json(new
            {
                success = false,
                message = "Dados inválidos. Por favor, verifique e tente novamente."
            });
        }

        [HttpGet]
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec_Esp")]
        public IActionResult DespromocaoMultipla()
        {
            if (!UsuarioTemAcessoArea("DPQ"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }

            ViewData["OrdemServicoId"] = new SelectList(_context.OrdemServicos, "Id", "NumOrdemServico");
            ViewData["PatenteId"] = new SelectList(_context.Patentes, "Id", "Posto");
            ViewData["EfectivoId"] = new SelectList(_context.Efectivos, "Id", "NomeCompleto");
            ViewData["PatenteAtual"] = _context.Efectivos
                .Include(e => e.Patente)
                .ToDictionary(e => e.Id, e => e.Patente.Posto);

            return PartialView("_Despromover");
        }

        [HttpPost]
        [Authorize(Policy = "Require_Admin_ChDepar")]
        public async Task<IActionResult> DespromocaoMultipla([FromBody] List<EfectivoOrdemServicoDTO> promocoesDto)
        {
            if (!UsuarioTemAcessoArea("DPQ"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            if (promocoesDto == null || !promocoesDto.Any())
                return BadRequest(new { success = false, message = "Nenhuma despromoção foi enviada." });

            var sucesso = new List<object>();
            var erros = new List<object>();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var dto in promocoesDto)
                {
                    try
                    {
                        bool jaExiste = await _context.EfectivoOrdemServicos
                            .AnyAsync(e => e.EfectivoId == dto.EfectivoId &&
                                           e.OrdemServicoId == dto.OrdemServicoId);

                        if (jaExiste)
                        {
                            erros.Add(new
                            {
                                efectivoId = dto.EfectivoId,
                                motivo = "Não pode ser despromovido nesta Ordem de Serviço."
                            });
                            continue;
                        }

                        var efectivo = await _context.Efectivos
                            .Include(e => e.Patente)
                            .FirstOrDefaultAsync(e => e.Id == dto.EfectivoId);

                        var novaPatente = await _context.Patentes.FindAsync(dto.PatenteId);
                        var ordemServico = await _context.OrdemServicos.FindAsync(dto.OrdemServicoId);

                        if (efectivo == null || novaPatente == null)
                        {
                            erros.Add(new
                            {
                                efectivoId = dto.EfectivoId,
                                motivo = "Efectivo ou patente não encontrados."
                            });
                            continue;
                        }

                        if (efectivo.Patente != null && efectivo.Patente.grau <= novaPatente.grau)
                        {
                            erros.Add(new
                            {
                                efectivoId = dto.EfectivoId,
                                nome = efectivo.NomeCompleto,
                                motivo = $"Patente nova ({novaPatente.Posto}) superior ou igual à ({efectivo.Patente.Posto})."
                            });
                            continue;
                        }

                        // Inserir promoção
                        var promocao = new EfectivoOrdemServico
                        {
                            EfectivoId = dto.EfectivoId,
                            OrdemServicoId = dto.OrdemServicoId,
                            PatenteId = dto.PatenteId,
                            NumDespacho = dto.NumDespacho,
                            DataRegisto = DateTime.Now,
                            DataUltimaAlterecao = DateTime.Now,
                            Estado = true
                        };
                        promocao.TipoPromocao = "d";
                        _context.EfectivoOrdemServicos.Add(promocao);

                        efectivo.PatenteId = dto.PatenteId;
                        _context.Efectivos.Update(efectivo);

                        _context.EfectivoHistoricos.Add(new EfectivoHistorico
                        {
                            EfectivoId = efectivo.Id,
                            Tipo = "Despromoção",
                            Descricao = $"Despromoção para a patente de {novaPatente?.Posto} via Ordem de Serviço Nº {ordemServico?.NumOrdemServico}",
                            Data = DateTime.Now,
                            Status = "Aprovado"
                        });

                        sucesso.Add(new
                        {
                            efectivoId = dto.EfectivoId,
                            nome = efectivo.NomeCompleto,
                            novaPatente = novaPatente.Posto
                        });
                    }
                    catch (Exception ex)
                    {
                        erros.Add(new
                        {
                            efectivoId = dto.EfectivoId,
                            motivo = "Erro interno: " + ex.Message
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    success = true,
                    mensagem = "Processamento concluído.",
                    promovidos = sucesso,
                    falhados = erros
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Erro ao processar promoções.", detalhes = ex.Message });
            }
        }

        // GET: Dpq/EfectivoOrdemServicoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!UsuarioTemAcessoArea("DPQ"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            if (id == null)
            {
                return NotFound();
            }

            var efectivoOrdemServico = await _context.EfectivoOrdemServicos
                .Include(e => e.Efectivo)
                .Include(e => e.Patente)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (efectivoOrdemServico == null)
            {
                return NotFound();
            }

            return View(efectivoOrdemServico);
        }

        // POST: Dpq/EfectivoOrdemServicoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!UsuarioTemAcessoArea("DPQ"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            var efectivoOrdemServico = await _context.EfectivoOrdemServicos.FindAsync(id);
            if (efectivoOrdemServico != null)
            {
                _context.EfectivoOrdemServicos.Remove(efectivoOrdemServico);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EfectivoOrdemServicoExists(int id)
        {
            return _context.EfectivoOrdemServicos.Any(e => e.Id == id);
        }
    }
}
