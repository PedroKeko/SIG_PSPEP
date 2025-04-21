using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidade;
using SIG_PSPEP.Entidades;
using SIG_PSPEP.Areas.Dpq.Models;
using SIG_PSPEP.Services;
using Microsoft.AspNetCore.Authorization;

namespace SIG_PSPEP.Areas.Dpq.Controllers
{
    [Area("Dpq")]
    [Authorize(Policy = "Require_Admin_ChDepar")]
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

        // GET: Efectivos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Efectivos.Include(e => e.FuncaoCargo).Include(e => e.Municipio).Include(e => e.OrgaoUnidade).Include(e => e.Patente).Include(e => e.ProvinciaNascimento).Include(e => e.ProvinciaResidencia).Include(e => e.SituacaoEfectivo).Include(e => e.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Efectivos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            CarregarViewData();

            var efectivo = await _context.Efectivos.FindAsync(id);
            if (efectivo == null)
            {
                return NotFound();
            }

            var foto = await _context.FotoEfectivos
                .Where(f => f.EfectivoId == id)
                .Select(f => f.Foto)
                .FirstOrDefaultAsync();

            var model = new EfectivoViewModel
            {
                Id = efectivo.Id,
                SituacaoEfectivoId = efectivo.SituacaoEfectivoId,
                OrgaoUnidadeId = efectivo.OrgaoUnidadeId,
                FuncaoCargoId = efectivo.FuncaoCargoId,
                PatenteId = efectivo.PatenteId,
                ProvinciaNascId = efectivo.ProvinciaNascId,
                ProvinciaResId = efectivo.ProvinciaResId,
                MunicipioId = efectivo.MunicipioId,
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
                FotoByte = foto
            };

            return PartialView("_Details", model);
        }

        public IActionResult Create()
        {
            CarregarViewData();
            return PartialView("_Create", new EfectivoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EfectivoViewModel model)
        {
            CarregarViewData();
            var existeNumeroProcesso = await _context.Efectivos
                .AnyAsync(e => e.Num_Processo == model.Num_Processo);

            if (existeNumeroProcesso)
            {
                ModelState.AddModelError("Num_Processo", "Este número de processo já está cadastrado.");
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Este número de processo já está cadastrado." });
            }

            //if (!ModelState.IsValid)
            //{
            //    return PartialView("_Create", model);
            //}

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "Usuário não autenticado.");
                //return PartialView("_Create", model);
                return Json(new { success = false, message = "Usuário não autenticado." });
            }

            try
            {
                var efectivo = new Efectivo
                {
                    SituacaoEfectivoId = model.SituacaoEfectivoId,
                    OrgaoUnidadeId = model.OrgaoUnidadeId,
                    FuncaoCargoId = model.FuncaoCargoId,
                    PatenteId = model.PatenteId,
                    ProvinciaNascId = model.ProvinciaNascId,
                    ProvinciaResId = model.ProvinciaResId,
                    MunicipioId = model.MunicipioId,
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
                    Estado = true,
                    DataRegisto = DateTime.Now,
                    DataUltimaAlterecao = DateTime.Now,
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
                    {
                        ModelState.AddModelError("FotoIF", "Formato de imagem inválido. Apenas JPG e PNG são permitidos.");
                        return PartialView("_Create", model);
                    }

                    if (model.FotoIF.Length > 5.5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("FotoIF", "O tamanho da imagem excede o limite de 5.5MB.");
                        return PartialView("_Create", model);
                    }

                    using (var imageStream = model.FotoIF.OpenReadStream())
                    {
                        fotoBytes = await _imageCompressionService.CompressImageAsync(imageStream, fileExtension);
                    }
                }

                var fotoEfectivo = new FotoEfectivo
                {
                    Foto = fotoBytes,
                    EfectivoId = efectivo.Id,
                    UserId = user.Id,
                    Estado = true,
                    DataRegisto = DateTime.Now,
                    DataUltimaAlterecao = DateTime.Now,
                };

                _context.FotoEfectivos.Add(fotoEfectivo);
                await _context.SaveChangesAsync();

                // Redireciona à Index via Ajax
                return Json(new { success = true, redirectUrl = Url.Action("Index") });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Erro ao salvar no banco de dados. Verifique os dados inseridos.");
                return Json(new { success = false, message = "Erro ao salvar no banco de dados. Verifique os dados inseridos." });
            }
        }

        // GET: Efectivos/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            CarregarViewData();

            var efectivo = await _context.Efectivos.FindAsync(id);
            if (efectivo == null)
            {
                return NotFound();
            }

            var foto = await _context.FotoEfectivos
                .Where(f => f.EfectivoId == id)
                .Select(f => f.Foto)
                .FirstOrDefaultAsync();

            var model = new EfectivoViewModel
            {
                Id = efectivo.Id,
                SituacaoEfectivoId = efectivo.SituacaoEfectivoId,
                OrgaoUnidadeId = efectivo.OrgaoUnidadeId,
                FuncaoCargoId = efectivo.FuncaoCargoId,
                PatenteId = efectivo.PatenteId,
                ProvinciaNascId = efectivo.ProvinciaNascId,
                ProvinciaResId = efectivo.ProvinciaResId,
                MunicipioId = efectivo.MunicipioId,
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
                FotoByte = foto
            };

            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EfectivoViewModel model)
        {
            CarregarViewData();

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dados inválidos. Corrija os erros e tente novamente." });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "Usuário não autenticado.");
                //return PartialView("_Create", model);
                return Json(new { success = false, message = "Usuário não autenticado." });
            }

            var efectivo = await _context.Efectivos.FindAsync(model.Id);
            if (efectivo == null)
            {
                return NotFound();
            }

            try
            {
                // Atualiza os dados do efectivo
                efectivo.SituacaoEfectivoId = model.SituacaoEfectivoId;
                efectivo.OrgaoUnidadeId = model.OrgaoUnidadeId;
                efectivo.FuncaoCargoId = model.FuncaoCargoId;
                efectivo.PatenteId = model.PatenteId;
                efectivo.ProvinciaNascId = model.ProvinciaNascId;
                efectivo.ProvinciaResId = model.ProvinciaResId;
                efectivo.MunicipioId = model.MunicipioId;
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
                efectivo.DataUltimaAlterecao = DateTime.Now;
                efectivo.UserId = user.Id;

                // Atualiza a foto (se necessário)
                byte[] fotoBytes = null;

                if (model.FotoIF != null && model.FotoIF.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(model.FotoIF.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("FotoIF", "Formato de imagem inválido.");
                        return PartialView("_Edit", model);
                    }

                    if (model.FotoIF.Length > 5.5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("FotoIF", "Imagem excede 5.5MB.");
                        return PartialView("_Edit", model);
                    }

                    using var stream = model.FotoIF.OpenReadStream();
                    fotoBytes = await _imageCompressionService.CompressImageAsync(stream, extension);
                }

                var fotoExistente = await _context.FotoEfectivos
                    .FirstOrDefaultAsync(f => f.EfectivoId == efectivo.Id);

                if (fotoBytes != null)
                {
                    if (fotoExistente != null)
                    {
                        fotoExistente.UserId = user.Id;
                        fotoExistente.Foto = fotoBytes;
                        fotoExistente.DataUltimaAlterecao = DateTime.Now;
                    }
                    else
                    {
                        _context.FotoEfectivos.Add(new FotoEfectivo
                        {
                            EfectivoId = efectivo.Id,
                            Foto = fotoBytes,
                            UserId = user?.Id,
                            Estado = true,
                            DataUltimaAlterecao = DateTime.Now
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, redirectUrl = Url.Action("Index") });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro ao editar o efectivo." });
            }
        }


        // GET: Efectivos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var efectivo = await _context.Efectivos
                .Include(e => e.FuncaoCargo)
                .Include(e => e.Municipio)
                .Include(e => e.OrgaoUnidade)
                .Include(e => e.Patente)
                .Include(e => e.ProvinciaNascimento)
                .Include(e => e.ProvinciaResidencia)
                .Include(e => e.SituacaoEfectivo)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (efectivo == null)
            {
                return NotFound();
            }

            return View(efectivo);
        }

        // POST: Efectivos/Delete/5
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

        private bool EfectivoExists(int id)
        {
            return _context.Efectivos.Any(e => e.Id == id);
        }

        private void CarregarViewData()
        {
            var efectivo = new Efectivo();
            ViewData["FuncaoCargoId"] = new SelectList(_context.FuncaoCargos, "Id", "NomeFuncaoCargo", efectivo.FuncaoCargoId);
            ViewData["OrgaoUnidadeId"] = new SelectList(_context.OrgaoUnidades, "Id", "NomeOrgaoUnidade", efectivo.OrgaoUnidadeId);
            ViewData["PatenteId"] = new SelectList(_context.Patentes, "Id", "Posto", efectivo.PatenteId);
            ViewData["SituacaoEfectivoId"] = new SelectList(_context.SituacaoEfectivos, "Id", "TipoSituacao", efectivo.SituacaoEfectivoId);
            ViewData["ProvinciaNascId"] = new SelectList(_context.Provincias, "Id", "Nome", efectivo.ProvinciaNascId);
            ViewData["ProvinciaResId"] = new SelectList(_context.Provincias, "Id", "Nome", efectivo.ProvinciaResId);
            ViewData["MunicipioId"] = new SelectList(_context.Municipios, "Id", "Nome", efectivo.MunicipioId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", efectivo.UserId);
        }
    }
}
