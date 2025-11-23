using FastReport;
using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Areas.Dpq.Models;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;
using SIG_PSPEP.Services;
using System.Data;

namespace SIG_PSPEP.Areas.Dpq.Controllers
{
    [Area("Dpq")]
    [Authorize(Policy = "Require_Admin_ChDepar_ChSec_Esp")]
    public class EfectivosController(
        IWebHostEnvironment _hostingEnvironment,
        AppDbContext context,
        UserManager<IdentityUser> _userManager,
        ImageCompressionService _imageCompressionService,
        SignInManager<IdentityUser> signInManager,
        ILogger<EfectivosController> logger // Corrigido aqui o nome
    ) : BaseController(context)
    {

        // GET: Efectivos  
        public async Task<IActionResult> Index()
        {
            if (!UsuarioTemAcessoArea("DPQ"))
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
                .OrderByDescending(e => e.DataRegisto);
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            // Busca o efectivo pelo ID e carrega todos os relacionamentos necessários
            var efectivo = await _context.Efectivos
                .Include(e => e.OrgaoUnidade)
                .Include(e => e.FuncaoCargo)
                .Include(e => e.Patente)
                .Include(e => e.ProvinciaNascimento)
                .Include(e => e.ProvinciaResidencia)
                .Include(e => e.Municipio)
                .Include(e => e.SituacaoEfectivo)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (efectivo == null)
            {
                return NotFound();
            }

            var historicos = await _context.EfectivoHistoricos
                .Where(h => h.EfectivoId == id)
                .OrderByDescending(h => h.Data)
                .ToListAsync();

            var foto = await _context.FotoEfectivos
                .Where(f => f.EfectivoId == id)
                .Select(f => f.Foto)
                .FirstOrDefaultAsync();

            int idade = DateTime.Today.Year - efectivo.DataNasc.Year;
            if (efectivo.DataNasc.Date > DateTime.Today.AddYears(-idade))
            {
                idade--;
            }

            int tempoServico = DateTime.Today.Year - efectivo.DataIngresso.Year;
            if (efectivo.DataIngresso.Date > DateTime.Today.AddYears(-tempoServico))
            {
                tempoServico--;
            }

            var model = new DetalhesEfetivoViewModel
            {
                Id = efectivo.Id,
                SituacaoEfectivo = efectivo.SituacaoEfectivo?.TipoSituacao ?? "Não definido",
                OrgaoUnidade = efectivo.OrgaoUnidade?.NomeOrgaoUnidade ?? "Não definido",
                SiglaUnidade = efectivo.OrgaoUnidade?.Sigla ?? "Não definido",
                FuncaoCargo = efectivo.FuncaoCargo?.NomeFuncaoCargo ?? "Não definido",
                Patente = efectivo.Patente?.Posto ?? "Não definido",
                ProvinciaNasc = efectivo.ProvinciaNascimento?.Nome ?? "Não definido",
                ProvinciaRes = efectivo.ProvinciaResidencia?.Nome ?? "Não definido",
                Municipio = efectivo.Municipio?.Nome ?? "Não definido",
                Num_Processo = efectivo.Num_Processo,
                NIP = efectivo.NIP,
                N_Agente = efectivo.N_Agente,
                NomeCompleto = efectivo.NomeCompleto,
                Apelido = efectivo.Apelido,
                Genero = efectivo.Genero,
                DataNasc = efectivo.DataNasc,
                Idade = idade,
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
                TempoServico = tempoServico,
                TipoVinculo = efectivo.TipoVinculo,
                Carreira = efectivo.Carreira,
                UnidadeOrigem = efectivo.UnidadeOrigem,
                OutrasInfo = efectivo.OutrasInfo,
                FotoByte = foto,
                Historicos = historicos
            };

            return PartialView("_Details", model);
        }

        public IActionResult Create()
        {
            if (!UsuarioTemAcessoArea("DPQ") && !UsuarioTemAcessoArea("ADMIN"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            CarregarViewData();
            return PartialView("_Create", new EfectivoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EfectivoViewModel model)
        {
            if (!UsuarioTemAcessoArea("DPQ"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            CarregarViewData();
            var existeNumeroProcesso = await _context.Efectivos
                .AnyAsync(e => e.Num_Processo == model.Num_Processo);
            var existeNIP = await _context.Efectivos
              .AnyAsync(e => e.NIP == model.NIP);
            var existeNunAgente = await _context.Efectivos
              .AnyAsync(e => e.N_Agente == model.N_Agente);

            // Validação de data de nascimento
            if (model.DataNasc == null ||
                model.DataNasc > DateTime.Now.AddYears(-18))
            {
                ModelState.AddModelError("DataNascimento", "O Efectivo deve ter 18 anos ou mais.");
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "O Efectivo deve ter 18 anos ou mais." });
            }

            // Validação do número de documento
            if (existeNumeroProcesso)
            {
                ModelState.AddModelError("Num_Processo", "Este número de processo já está cadastrado.");
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Este número de processo já está cadastrado." });
            }

            // Validação do NIP
            if (existeNIP)
            {
                ModelState.AddModelError("NIP", "Este NIP não pertence a este Efectivo, já existe!");
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Este NIP não pertence a este Efectivo, já existe!" });
            }

            // Validação do número de Agente
            if (existeNunAgente)
            {
                ModelState.AddModelError("N_Agente", "Este número de Agente já existe!");
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Este número de Agente já existe!" });
            }

            // Validação do número de documento
            if (_context.Efectivos.Any(c => c.NumBI == model.NumBI))
            {
                ModelState.AddModelError("NumeroDocumento", "Já existe um efectivo com este número de BI.");
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Já existe um efectivo com este número de BI." });
            }

            // Validação do Email
            if (!string.IsNullOrEmpty(model.Email) && _context.Users.Any(u => u.Email == model.Email))
            {
                return Json(new { success = false, message = "Já existe um usuário com este email." });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "Usuário não autenticado.");
                //return PartialView("_Create", model);
                return Json(new { success = false, message = "Usuário não autenticado." });
            }

            try
            {
                // 2. Base do email: PrimeiroNome + UltimoNome (sem espaços)
                var nomes = model.NomeCompleto
                    .Trim()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                string primeiroNome = nomes.First().ToLower();
                string ultimoNome = nomes.Last().ToLower();

                string baseNome = primeiroNome + ultimoNome;

                string dominio = "@pspep.pn.ao";
                string emailFinal = baseNome + dominio;
                int contador = 1;

                // 3. Garante que o email seja único
                while (await _userManager.FindByEmailAsync(emailFinal) != null)
                {
                    emailFinal = baseNome + contador + dominio;
                    contador++;
                }
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
                    Email = model.Email ?? emailFinal,
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

                // 4. Cria o IdentityUser
                var usuario = new IdentityUser
                {
                    UserName = model.Email ?? emailFinal,
                    Email = model.Email ?? emailFinal,
                    EmailConfirmed = true
                };

                if (!string.IsNullOrEmpty(model.Telefone1))
                {
                    usuario.PhoneNumber = model.Telefone1;
                }

                var senha = "Pspep#12345";
                var result = await _userManager.CreateAsync(usuario, senha);

                if (result.Succeeded)
                {
                    // 4. Garante que a Role "COMUN" exista
                    var roleManager = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
                    if (!await roleManager.RoleExistsAsync("Usuario Comum"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("Usuario Comum"));
                    }

                    await _userManager.AddToRoleAsync(usuario, "Usuario Comum");

                    var areaId = _context.Areas
                       .Where(a => a.NomeArea == "Portal")
                       .Select(a => a.Id).FirstOrDefault();

                    if (areaId == null)
                    {
                        ModelState.AddModelError("", "Área não localizada.");
                        //return PartialView("_Create", model);
                        return Json(new { success = false, message = "Área não localizada!" });
                    }

                    // 4. Cria vínculo na tabela UsuarioAute
                    var usuarioAute = new UsuarioAute
                    {
                        UserId = usuario.Id,
                        AreaId = areaId,
                        EfectivoId = efectivo.Id
                    };

                    _context.UsuarioAutes.Add(usuarioAute);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Pode exibir erros na view
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", "Usuário não foi gerado infelizmente.");
                        //return PartialView("_Create", model);
                        return Json(new { success = false, message = "Usuário não foi gerado infelizmente." });
                    }
                }

                // Redireciona à Index via Ajax
                return Json(new
                {
                    success = true,
                    message = "Efectivo Cadastrado com Sucesso!",
                    redirectUrl = Url.Action("Index")
                });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Erro ao salvar no banco de dados. Verifique os dados inseridos.");
                return Json(new { success = false, message = "Erro ao salvar no banco de dados. Verifique os dados inseridos." });
            }
        }

        // GET: Efectivos/Edit/5
        [HttpGet]
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec")]
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
            if (!UsuarioTemAcessoArea("DPQ"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
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

        //Eliminar Registro
        [HttpPost]
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!UsuarioTemAcessoArea("DPQ"))
            {
                return Forbid(); // ou RedirectToAction("AcessoNegado", "Conta");
            }
            try
            {
                var efetivo = await _context.Efectivos.FindAsync(id);

                if (efetivo == null)
                {
                    return NotFound();
                }

                // 1. Verifica se o efectivo está vinculado ao usuário logado
                var usuarioLogado = await _userManager.GetUserAsync(User);
                var usuarioAute = await _context.UsuarioAutes
                                                .FirstOrDefaultAsync(ua => ua.EfectivoId == id);

                if (usuarioAute != null && usuarioLogado != null && usuarioAute.UserId == usuarioLogado.Id)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Não é permitido eliminar o seu próprio registo."
                    });
                }

                // 2. Validação: não pode eliminar se já passaram mais de 2 horas
                if (efetivo.DataRegisto != null && efetivo.DataRegisto.AddHours(2) < DateTime.UtcNow)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"O registo criado em {efetivo.DataRegisto:dd/MM/yyyy HH:mm} não pode ser eliminado após 2 horas."
                    });
                }

                // 3. Remover fotos associadas
                var fotoefetivo = await _context.FotoEfectivos
                                                .Where(fc => fc.EfectivoId == id)
                                                .ToListAsync();

                if (fotoefetivo.Any())
                {
                    _context.FotoEfectivos.RemoveRange(fotoefetivo);
                }

                // 4. Buscar e remover o usuário associado
                IdentityUser? user = null;
                if (usuarioAute != null)
                {
                    user = await _userManager.FindByIdAsync(usuarioAute.UserId);
                    _context.UsuarioAutes.Remove(usuarioAute);
                }

                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Any())
                        await _userManager.RemoveFromRolesAsync(user, roles);

                    var claims = await _userManager.GetClaimsAsync(user);
                    if (claims.Any())
                    {
                        foreach (var claim in claims)
                            await _userManager.RemoveClaimAsync(user, claim);
                    }

                    var deleteResult = await _userManager.DeleteAsync(user);
                    if (!deleteResult.Succeeded)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Erro ao remover o usuário associado ao efectivo.",
                            errors = deleteResult.Errors.Select(e => e.Description)
                        });
                    }
                }

                // 5. Remover o efectivo
                _context.Efectivos.Remove(efetivo);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = $"Efectivo {efetivo.NomeCompleto} removido com sucesso.",
                    redirectUrl = Url.Action("Index")
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Erro inesperado ao eliminar o efectivo.",
                    errorDetails = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        public JsonResult GetMunicipios(int provinciaId)
        {
            var municipios = _context.Municipios
                .Where(m => m.ProvinciaId == provinciaId)
                .Select(m => new { m.Id, m.Nome })
                .ToList();

            return Json(municipios);
        }

        private void CarregarViewData()
        {
            var efectivo = new Efectivo();
            ViewData["FuncaoCargoId"] = new SelectList(_context.FuncaoCargos, "Id", "NomeFuncaoCargo", efectivo.FuncaoCargoId);
            ViewData["OrgaoUnidadeId"] = new SelectList(_context.OrgaoUnidades.Select(o => 
            new{o.Id,NomeCompleto = o.NomeOrgaoUnidade + " (" + o.Sigla + ")"}), 
            "Id","NomeCompleto", efectivo.OrgaoUnidadeId);
            ViewData["OrgUnidPnaMinints"] = new SelectList(_context.OrgUnidPnaMinints.Select(o => 
            new{o.Id, NomeOrgUnid = o.NomeOrgaoUnidade + " (" + o.Sigla + ")"}),
            "NomeOrgUnid", "NomeOrgUnid", efectivo.OrgaoUnidadeId);
            ViewData["PatenteId"] = new SelectList(_context.Patentes, "Id", "Posto", efectivo.PatenteId);
            ViewData["SituacaoEfectivoId"] = new SelectList(_context.SituacaoEfectivos, "Id", "TipoSituacao", efectivo.SituacaoEfectivoId);
            ViewData["ProvinciaNascId"] = new SelectList(_context.Provincias, "Id", "Nome", efectivo.ProvinciaNascId);
            ViewData["ProvinciaResId"] = new SelectList(_context.Provincias, "Id", "Nome", efectivo.ProvinciaResId);
            ViewData["MunicipioId"] = new SelectList(_context.Municipios, "Id", "Nome", efectivo.MunicipioId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", efectivo.UserId);
        }
        //public async Task<IActionResult> GerarPasse(int id)
        //{
        //    // 1. Carrega o efectivo e a foto
        //    var efectivo = await _context.Efectivos
        //        .Include(e => e.OrgaoUnidade)
        //        .Include(e => e.FuncaoCargo)
        //        .Include(e => e.Patente)
        //        .FirstOrDefaultAsync(e => e.Id == id);

        //    if (efectivo == null)
        //        return NotFound("Efetivo não encontrado.");

        //    var foto = await _context.FotoEfectivos
        //        .Where(f => f.EfectivoId == id)
        //        .Select(f => f.Foto)
        //        .FirstOrDefaultAsync();

        //    // 2. Carrega o relatório .frx
        //    var reportPath = Path.Combine(_hostingEnvironment.WebRootPath, "Reports", "PasseIdentificacao.frx");
        //    if (!System.IO.File.Exists(reportPath))
        //        return NotFound("Relatório não encontrado.");

        //    using var report = new Report();
        //    report.Load(reportPath);

        //    // 3. Atribui valores aos parâmetros
        //    report.SetParameterValue("SiglaUnidade", efectivo.OrgaoUnidade?.Sigla ?? "");
        //    report.SetParameterValue("FuncaoCargo", efectivo.FuncaoCargo?.NomeFuncaoCargo ?? "");
        //    report.SetParameterValue("Classe", efectivo.Patente?.Classe ?? "");
        //    report.SetParameterValue("Patente", efectivo.Patente?.Posto ?? "");
        //    report.SetParameterValue("Num_Processo", efectivo.Num_Processo ?? "");
        //    report.SetParameterValue("NomeCompleto", efectivo.NomeCompleto);
        //    report.SetParameterValue("FotoByte", foto ?? new byte[0]);

        //    // 4. Prepara e exporta para PDF
        //    report.Prepare();
        //    using var ms = new MemoryStream();
        //    var pdfExport = new PDFSimpleExport();
        //    pdfExport.Export(report, ms);
        //    ms.Position = 0;

        //    // 5. Retorna o PDF
        //    var fileName = $"Passe_{efectivo.NomeCompleto.Replace(" ", "_")}.pdf";
        //    return File(ms.ToArray(), "application/pdf", fileName);
        //}

        [HttpGet]
        public async Task<IActionResult> GerarPasse(int id)
        {
            try
            {
                var efectivo = await _context.Efectivos
                    .Include(e => e.OrgaoUnidade)
                    .Include(e => e.FuncaoCargo)
                    .Include(e => e.Patente)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (efectivo == null)
                    return NotFound("Efetivo não encontrado.");

                var foto = await _context.FotoEfectivos
                    .Where(f => f.EfectivoId == id)
                    .Select(f => f.Foto)
                    .FirstOrDefaultAsync();

                var dataSet = new DataSet();
                var table = new DataTable("Efectivo");

                table.Columns.Add("NomeCompleto", typeof(string));
                table.Columns.Add("Num_Processo", typeof(string));
                table.Columns.Add("SiglaUnidade", typeof(string));
                table.Columns.Add("FuncaoCargo", typeof(string));
                table.Columns.Add("Classe", typeof(string));
                table.Columns.Add("Patente", typeof(string));
                table.Columns.Add("FotoByte", typeof(byte[]));

                table.Rows.Add(
                    efectivo.NomeCompleto ?? "",
                    efectivo.Num_Processo ?? "",
                    efectivo.OrgaoUnidade?.Sigla ?? "",
                    efectivo.FuncaoCargo?.NomeFuncaoCargo ?? "",
                    efectivo.Patente?.Classe ?? "",
                    efectivo.Patente?.Posto ?? "",
                    foto ?? new byte[0]
                );

                dataSet.Tables.Add(table);

                var reportPath = Path.Combine(_hostingEnvironment.WebRootPath, "Reports", "Passe.frx");
                if (!System.IO.File.Exists(reportPath))
                    return NotFound($"Relatório não encontrado em: {reportPath}");

                using var report = new Report();
                report.Load(reportPath);
                report.RegisterData(dataSet, "Data");
                var dataSource = report.GetDataSource("Data.Efectivo") as TableDataSource;
                if (dataSource != null)dataSource.Enabled = true;
                report.Prepare();

                using var ms = new MemoryStream();
                var pdfExport = new PDFSimpleExport();
                pdfExport.Export(report, ms);
                ms.Position = 0;

                var fileName = $"Passe_{efectivo.NomeCompleto.Replace(" ", "_")}.pdf";
                return File(ms.ToArray(), "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao gerar o passe: {ex.Message}");
            }
        }


        public async Task<IActionResult> CriarModeloPasseIdentificacao()
        {
            // 1. Caminho onde será salvo o .frx
            var reportPath = Path.Combine(_hostingEnvironment.WebRootPath, "Reports", "Passe.frx");

            // Cria o diretório se não existir
            Directory.CreateDirectory(Path.GetDirectoryName(reportPath));

            // 2. Criação do relatório
            using Report report = new Report();

            // 3. Criação do DataSet manualmente (modelo da entidade Efectivo com campos principais)
            var dataSet = new DataSet();
            var table = new DataTable("Efectivo");

            table.Columns.Add("NomeCompleto", typeof(string));
            table.Columns.Add("Num_Processo", typeof(string));
            table.Columns.Add("SiglaUnidade", typeof(string));
            table.Columns.Add("FuncaoCargo", typeof(string));
            table.Columns.Add("Classe", typeof(string));
            table.Columns.Add("Patente", typeof(string));
            table.Columns.Add("FotoByte", typeof(byte[]));

            dataSet.Tables.Add(table);
            report.RegisterData(dataSet, "Data");

            // 4. Habilita o DataSource para uso no designer
            var registeredTable = report.GetDataSource("Efectivo") as TableDataSource;
            if (registeredTable != null)
                registeredTable.Enabled = true;

            // 5. Salva o arquivo .frx
            report.Save(reportPath);

            return Ok("Relatório criado com sucesso em: " + reportPath);
        }
    }
}