using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;
using SIG_PSPEP.Services;
using SIG_PSPEP.Models;

namespace SIG_PSPEP.Areas.Dpq.Controllers
{
    [Area("Dpq")]
    public class EfectivosController : Controller
    {
        private readonly ImageCompressionService _imageCompressionService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public EfectivosController(AppDbContext context, UserManager<IdentityUser> userManager, ImageCompressionService imageCompressionService)
        {
            _context = context;
            _userManager = userManager;
            _imageCompressionService = imageCompressionService;
        }

        // GET: Dpq/Efectivos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Efectivos.Include(e => e.FuncaoCargo).Include(e => e.OrgaoUnidade).Include(e => e.Patente).Include(e => e.SituacaoEfectivo).Include(e => e.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Dpq/Efectivos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var efectivo = await _context.Efectivos
                .Include(e => e.FuncaoCargo)
                .Include(e => e.OrgaoUnidade)
                .Include(e => e.Patente)
                .Include(e => e.SituacaoEfectivo)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (efectivo == null)
            {
                return NotFound();
            }

            return View(efectivo);
        }

        // GET: Dpq/Efectivos/Create
        public IActionResult Create()
        {
            ViewData["FuncaoCargoId"] = new SelectList(_context.FuncaoCargos, "Id", "NomeFuncaoCargo");
            ViewData["OrgaoUnidadeId"] = new SelectList(_context.OrgaoUnidades, "Id", "NomeOrgaoUnidade");
            ViewData["PatenteId"] = new SelectList(_context.Patentes, "Id", "Posto");
            ViewData["SituacaoEfectivoId"] = new SelectList(_context.SituacaoEfectivos, "Id", "TipoSituacao");

            return PartialView("Create", new EfectivoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EfectivoViewModel model)
        {
            // Verifica se o número de processo já existe
            var existeNumeroProcesso = await _context.Efectivos
                .AnyAsync(e => e.Num_Processo == model.Num_Processo);

            if (existeNumeroProcesso)
            {
                return Json(new { success = false, errors = new List<string> { "Este número de processo já está cadastrado." } });
            }

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return Json(new { success = false, errors = erros });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, errors = new List<string> { "Usuário não autenticado." } });
            }

            try
            {
                // Criar entidade Efectivo
                var efectivo = new Efectivo
                {
                    SituacaoEfectivoId = model.SituacaoEfectivoId,
                    OrgaoUnidadeId = model.OrgaoUnidadeId,
                    FuncaoCargoId = model.FuncaoCargoId,
                    PatenteId = model.PatenteId,
                    Num_Processo = model.Num_Processo,
                    NIP = model.NIP,
                    N_Agente = model.N_Agente,
                    NomeCompleto = model.NomeCompleto,
                    Apelido = model.Apelido,
                    Genero = model.Genero,
                    DataNasc = model.DataNasc,
                    EstadoCivil = model.EstadoCivil,
                    GSanguineo = model.GSanguineo,
                    NumBI = model.NumBI,
                    BIValidade = model.BIValidade,
                    BIEmitido = model.BIEmitido,
                    NumCartaConducao = model.NumCartaConducao,
                    CartaValidade = model.CartaValidade,
                    CartaEmitido = model.CartaEmitido,
                    NumPassaporte = model.NumPassaporte,
                    PassapValidade = model.PassapValidade,
                    PassapEmitido = model.PassapEmitido,
                    Nacionalidade = model.Nacionalidade,
                    Naturalidade = model.Naturalidade,
                    MunicipioRes = model.MunicipioRes,
                    Destrito_BairroRes = model.Destrito_BairroRes,
                    Rua = model.Rua,
                    CasaNum = model.CasaNum,
                    Habilitacao = model.Habilitacao,
                    CursoHabilitado = model.CursoHabilitado,
                    InstitAcademica = model.InstitAcademica,
                    Telefone1 = model.Telefone1,
                    Telefone2 = model.Telefone2,
                    Email = model.Email,
                    DataIngresso = model.DataIngresso,
                    TipoVinculo = model.TipoVinculo,
                    Carreira = model.Carreira,
                    UnidadeOrigem = model.UnidadeOrigem,
                    OutrasInfo = model.OutrasInfo,
                    UserId = user.Id,
                };

                _context.Efectivos.Add(efectivo);
                await _context.SaveChangesAsync();

                byte[] fotoBytes;
                if (model.FotoIF == null || model.FotoIF.Length == 0)
                {
                    fotoBytes = await _imageCompressionService.GetDefaultImageAsync();
                }
                else
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(model.FotoIF.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                        return Json(new { success = false, errors = new List<string> { "Formato de imagem inválido. Apenas JPG e PNG são permitidos." } });

                    if (model.FotoIF.Length > 5.5 * 1024 * 1024)
                        return Json(new { success = false, errors = new List<string> { "O tamanho da imagem excede o limite de 5.5MB." } });

                    using (var imageStream = model.FotoIF.OpenReadStream())
                    {
                        fotoBytes = await _imageCompressionService.CompressImageAsync(imageStream, fileExtension);
                    }
                }

                // Criar registro da foto no banco de dados
                var fotoEfectivo = new FotoEfectivo
                {
                    Foto = fotoBytes,
                    EfectivoId = efectivo.Id
                };

                _context.FotoEfectivos.Add(fotoEfectivo);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (DbUpdateException  dbEx)
            {
                return Json(new { success = false, errors = new List<string> { "Erro ao salvar no banco de dados. Verifique os dados inseridos." } });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new List<string> { "Erro inesperado. Tente novamente. " + ex.Message } });
            }
        }

        private void CarregarViewData()
        {
            var efectivo = new Efectivo();
            ViewData["FuncaoCargoId"] = new SelectList(_context.FuncaoCargos, "Id", "NomeFuncaoCargo", efectivo.FuncaoCargoId);
            ViewData["OrgaoUnidadeId"] = new SelectList(_context.OrgaoUnidades, "Id", "NomeOrgaoUnidade", efectivo.OrgaoUnidadeId);
            ViewData["PatenteId"] = new SelectList(_context.Patentes, "Id", "Posto", efectivo.PatenteId);
            ViewData["SituacaoEfectivoId"] = new SelectList(_context.SituacaoEfectivos, "Id", "TipoSituacao", efectivo.SituacaoEfectivoId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", efectivo.UserId);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var efectivo = await _context.Efectivos.FindAsync(id);
            if (efectivo == null)
            {
                return NotFound();
            }

            var fotoCondutor = await _context.FotoEfectivos
                .FirstOrDefaultAsync(f => f.EfectivoId == efectivo.Id);

            var model = new EfectivoViewModel
            {
                Id = efectivo.Id,
                Num_Processo = efectivo.Num_Processo,
                NIP = efectivo.NIP,
                N_Agente = efectivo.N_Agente,
                NomeCompleto = efectivo.NomeCompleto,
                Apelido = efectivo.Apelido,
                Genero = efectivo.Genero,
                DataNasc = efectivo.DataNasc,
                EstadoCivil = efectivo.EstadoCivil,
                GSanguineo = efectivo.GSanguineo,
                NumBI = efectivo.NumBI,
                BIValidade = efectivo.BIValidade,
                BIEmitido = efectivo.BIEmitido,
                NumCartaConducao = efectivo.NumCartaConducao,
                CartaValidade = efectivo.CartaValidade,
                CartaEmitido = efectivo.CartaEmitido,
                NumPassaporte = efectivo.NumPassaporte,
                PassapValidade = efectivo.PassapValidade,
                PassapEmitido = efectivo.PassapEmitido,
                Nacionalidade = efectivo.Nacionalidade,
                Naturalidade = efectivo.Naturalidade,
                MunicipioRes = efectivo.MunicipioRes,
                Destrito_BairroRes = efectivo.Destrito_BairroRes,
                Rua = efectivo.Rua,
                CasaNum = efectivo.CasaNum,
                Habilitacao = efectivo.Habilitacao,
                CursoHabilitado = efectivo.CursoHabilitado,
                InstitAcademica = efectivo.InstitAcademica,
                Telefone1 = efectivo.Telefone1,
                Telefone2 = efectivo.Telefone2,
                Email = efectivo.Email,
                DataIngresso = efectivo.DataIngresso,
                TipoVinculo = efectivo.TipoVinculo,
                Carreira = efectivo.Carreira,
                UnidadeOrigem = efectivo.UnidadeOrigem,
                OutrasInfo = efectivo.OutrasInfo,
                FotoByte = fotoCondutor?.Foto // ✅ Corrigido para evitar erro de `null`
            }; 
            CarregarViewData();

            return PartialView("Edit", model); // ✅ Verifique se a view está correta
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EfectivoViewModel model)
        {
            if (id != model.Id)
            {
                return Json(new { success = false, errors = new List<string> { "ID inválido." } });
            }

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errors = erros });
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "Usuário não autenticado.");
                ViewData["ValidationErrors"] = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                CarregarViewData();
                return View(model);
            }

            var efectivo = await _context.Efectivos.FindAsync(id);
            if (efectivo == null)
            {
                return Json(new { success = false, errors = new List<string> { "Efectivo não encontrado." } });
            }

            try
            {
                // Atualizar os dados do efectivo
                efectivo.SituacaoEfectivoId = model.SituacaoEfectivoId;
                efectivo.OrgaoUnidadeId = model.OrgaoUnidadeId;
                efectivo.FuncaoCargoId = model.FuncaoCargoId;
                efectivo.PatenteId = model.PatenteId;
                efectivo.Num_Processo = model.Num_Processo;
                efectivo.NIP = model.NIP;
                efectivo.N_Agente = model.N_Agente;
                efectivo.NomeCompleto = model.NomeCompleto;
                efectivo.Apelido = model.Apelido;
                efectivo.Genero = model.Genero;
                efectivo.DataNasc = model.DataNasc;
                efectivo.EstadoCivil = model.EstadoCivil;
                efectivo.GSanguineo = model.GSanguineo;
                efectivo.NumBI = model.NumBI;
                efectivo.BIValidade = model.BIValidade;
                efectivo.BIEmitido = model.BIEmitido;
                efectivo.NumCartaConducao = model.NumCartaConducao;
                efectivo.CartaValidade = model.CartaValidade;
                efectivo.CartaEmitido = model.CartaEmitido;
                efectivo.NumPassaporte = model.NumPassaporte;
                efectivo.PassapValidade = model.PassapValidade;
                efectivo.PassapEmitido = model.PassapEmitido;
                efectivo.Nacionalidade = model.Nacionalidade;
                efectivo.Naturalidade = model.Naturalidade;
                efectivo.MunicipioRes = model.MunicipioRes;
                efectivo.Destrito_BairroRes = model.Destrito_BairroRes;
                efectivo.Rua = model.Rua;
                efectivo.CasaNum = model.CasaNum;
                efectivo.Habilitacao = model.Habilitacao;
                efectivo.CursoHabilitado = model.CursoHabilitado;
                efectivo.InstitAcademica = model.InstitAcademica;
                efectivo.Telefone1 = model.Telefone1;
                efectivo.Telefone2 = model.Telefone2;
                efectivo.Email = model.Email;
                efectivo.DataIngresso = model.DataIngresso;
                efectivo.TipoVinculo = model.TipoVinculo;
                efectivo.Carreira = model.Carreira;
                efectivo.UnidadeOrigem = model.UnidadeOrigem;
                efectivo.OutrasInfo = model.OutrasInfo;
                efectivo.UserId = user.Id;
                efectivo.DataUltimaAlterecao = DateTime.Now;

                _context.Update(efectivo);
                await _context.SaveChangesAsync();

                // Atualizar ou inserir foto
                if (model.FotoIF != null && model.FotoIF.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(model.FotoIF.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                        return Json(new { success = false, errors = new List<string> { "Formato de imagem inválido. Apenas JPG e PNG são permitidos." } });

                    if (model.FotoIF.Length > 5.5 * 1024 * 1024)
                        return Json(new { success = false, errors = new List<string> { "O tamanho da imagem excede o limite de 5.5MB." } });

                    byte[] fotoBytes;
                    using (var imageStream = model.FotoIF.OpenReadStream())
                    {
                        fotoBytes = await _imageCompressionService.CompressImageAsync(imageStream, fileExtension);
                    }

                    var fotoEfectivo = await _context.FotoEfectivos.FirstOrDefaultAsync(f => f.EfectivoId == efectivo.Id);
                    if (fotoEfectivo == null)
                    {
                        fotoEfectivo = new FotoEfectivo { EfectivoId = efectivo.Id, Foto = fotoBytes };
                        _context.FotoEfectivos.Add(fotoEfectivo);
                    }
                    else
                    {
                        fotoEfectivo.Foto = fotoBytes;
                        _context.FotoEfectivos.Update(fotoEfectivo);
                    }

                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Efectivos.Any(e => e.Id == id))
                {
                    return Json(new { success = false, errors = new List<string> { "Efectivo não encontrado." } });
                }
                else
                {
                    return Json(new { success = false, errors = new List<string> { "Erro de concorrência ao atualizar os dados." } });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new List<string> { "Erro inesperado. Tente novamente. " + ex.Message } });
            }
        }

        private bool EfectivoExists(int id)
        {
            return _context.Efectivos.Any(e => e.Id == id);
        }



        // GET: Dpq/Efectivos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var efectivo = await _context.Efectivos
                .Include(e => e.FuncaoCargo)
                .Include(e => e.OrgaoUnidade)
                .Include(e => e.Patente)
                .Include(e => e.SituacaoEfectivo)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (efectivo == null)
            {
                return NotFound();
            }

            return View(efectivo);
        }

        // POST: Dpq/Efectivos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var efectivo = await _context.Efectivos.FindAsync(id);
            if (efectivo != null)
            {
                _context.Efectivos.Remove(efectivo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
