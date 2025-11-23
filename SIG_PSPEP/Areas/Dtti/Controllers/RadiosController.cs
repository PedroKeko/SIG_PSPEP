using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Controllers;
using SIG_PSPEP.Entidades;
using SIG_PSPEP.Services;

namespace SIG_PSPEP.Areas.Dtti.Controllers
{
    [Area("Dtti")]
    [Authorize(Policy = "Require_Admin_ChDepar_ChSec_Esp")]
    public class RadiosController(
            IWebHostEnvironment _hostingEnvironment,
        AppDbContext context,
        UserManager<IdentityUser> _userManager,
        ImageCompressionService _imageCompressionService,
        SignInManager<IdentityUser> signInManager,
        ILogger<RadiosController> logger // Corrigido aqui o nome
    ) : BaseController(context)
    {
        // GET: Dtti/Radios
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Radios.Include(r => r.RadioTipo).Include(r => r.User);
            return View(await appDbContext.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Json(new
                {
                    success = false,
                    message = "O identificador do registo é inválido."
                });
            }

            var radio = await _context.Radios
                .Include(r => r.RadioTipo)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (radio == null)
            {
                return Json(new
                {
                    success = false,
                    message = "O registo solicitado não foi encontrado."
                });
            }

            // Retorna o conteúdo do modal (HTML parcial)
            return PartialView("_Details", radio);
        }

        // GET: Dtti/Radios/Create
        public IActionResult Create()
        {
            ViewData["RadioTipoId"] = new SelectList(
                _context.RadioTipos.Select(r => new
                {
                    r.Id,
                    NomeCompleto = r.Marca + " - " + r.Modelo
                }),
                "Id",
                "NomeCompleto"
            );

            // Lista de opções do Estado Técnico
            ViewData["EstadoTecnicoList"] = new SelectList(new List<string>
            { 
                "Funcional", 
                "Avariado",
                "Sem Sinal",
                "Em Manutenção",
                "Desativado"
            });

            return PartialView("_Create", new Radio());
        }

        // POST: Dtti/Radios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Radio radio)
        {
            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage)
                                             .ToList();
                return Json(new { success = false, errors = erros });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Usuário não autenticado." });

            try
            {
                // 🔹 Gera o Código único automaticamente
                radio.CodRadio = await GerarCodigoUnicoAsync();

                // 🔹 Define dados automáticos
                radio.UserId = user.Id;
                radio.Estado = true;
                radio.DataRegisto = DateTime.Now;
                radio.DataUltimaAlterecao = DateTime.Now;

                // 🔹 Verificar duplicidades (TEI, NumSerie e IdRadio)
                var duplicado = await _context.Radios
                    .Where(r => r.IdRadio == radio.IdRadio
                             || r.TEI == radio.TEI
                             || r.NumSerie == radio.NumSerie)
                    .FirstOrDefaultAsync();

                if (duplicado != null)
                {
                    // Dicionário de campos a validar
                    var campos = new Dictionary<string, (string ValorDuplicado, string ValorAtual)>
                    {
                        { "ID", (duplicado.IdRadio?.ToString(), radio.IdRadio?.ToString()) },
                        { "TEI", (duplicado.TEI, radio.TEI) },
                        { "Série", (duplicado.NumSerie, radio.NumSerie) }
                    };

                    foreach (var campo in campos)
                    {
                        var nomeCampo = campo.Key;
                        var (valorDuplicado, valorAtual) = campo.Value;

                        if (string.IsNullOrWhiteSpace(valorAtual))
                        {
                            ModelState.AddModelError(nomeCampo, $"O número de {nomeCampo} é obrigatório!");
                        }
                        else if (valorDuplicado == valorAtual)
                        {
                            ModelState.AddModelError(nomeCampo, $"Este número de {nomeCampo} já existe!");
                        }
                    }

                    // Retorna erros se houver
                    if (!ModelState.IsValid)
                    {
                        var erros = ModelState.Values
                                              .SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                        return Json(new { success = false, errors = erros });
                    }
                }

                _context.Radios.Add(radio);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = $"Rádio criado com sucesso! Código gerado: {radio.CodRadio}",
                    redirectUrl = Url.Action("Index")
                });
            }
            catch (Exception ex)
            {
                // (Opcional) Logar erro ex.Message
                return Json(new
                {
                    success = false,
                    message = "Erro ao criar o registro. Verifique os dados inseridos."
                });
            }
        }

        #region Função Geradora de Código Único
        private async Task<string> GerarCodigoUnicoAsync()
        {
            var ano = DateTime.Now.Year;
            var ultimo = await _context.Radios
                .Where(r => r.DataRegisto.Year == ano)
                .OrderByDescending(r => r.CodRadio)
                .Select(r => r.CodRadio)
                .FirstOrDefaultAsync();

            int numeroSequencial = 1;

            if (!string.IsNullOrEmpty(ultimo))
            {
                var partes = ultimo.Split('-');
                if (partes.Length == 2 && int.TryParse(partes[0], out int numero))
                {
                    numeroSequencial = numero + 1;
                }
            }

            // 🔹 Gera no formato 00001-2025
            string novoCodigo = $"{numeroSequencial:D5}-{ano}";

            // 🔒 Garante que o código é único (loop até não encontrar duplicado)
            while (await _context.Radios.AnyAsync(r => r.CodRadio == novoCodigo))
            {
                numeroSequencial++;
                novoCodigo = $"{numeroSequencial:D5}-{ano}";
            }

            return novoCodigo;
        }
        #endregion

        // GET: Dtti/Radios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var radio = await _context.Radios.FindAsync(id);
            if (radio == null)
                return NotFound();

            ViewData["RadioTipoId"] = new SelectList(
                _context.RadioTipos.Select(r => new
                {
                    r.Id,
                    NomeCompleto = r.Marca + " - " + r.Modelo
                }),
                "Id",
                "NomeCompleto",
                radio.RadioTipoId
            );

            // Lista de opções do Estado Técnico
            ViewData["EstadoTecnicoList"] = new SelectList(new List<string>
            {
                "Funcional",
                "Avariado",
                "Sem Sinal",
                "Em Manutenção",
                "Desativado"
            });

            return PartialView("_Edit", radio);
        }

        // POST: Dtti/Radios/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Radio radio)
        {
            if (id != radio.Id)
                return Json(new { success = false, message = "Rádio inválido ou não encontrado." });

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage)
                                             .ToList();
                return Json(new { success = false, errors = erros });
            }

            var radioExistente = await _context.Radios.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (radioExistente == null)
                return Json(new { success = false, message = "Rádio não encontrado no banco de dados." });

            // 🔎 Validação de duplicidade (excluindo o próprio registro)
            var duplicado = await _context.Radios
                .Where(r => r.Id != id && (
                    r.IdRadio == radio.IdRadio ||
                    r.TEI == radio.TEI ||
                    r.NumSerie == radio.NumSerie))
                .FirstOrDefaultAsync();

            if (duplicado != null)
            {
                if (duplicado.IdRadio == radio.IdRadio)
                    ModelState.AddModelError("IdRadio", "Este número de ID já existe!");
                if (duplicado.TEI == radio.TEI)
                    ModelState.AddModelError("TEI", "Este número de TEI já existe!");
                if (duplicado.NumSerie == radio.NumSerie)
                    ModelState.AddModelError("NumSerie", "Este número de série já existe!");

                var erros = ModelState.Values.SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage)
                                             .ToList();
                return Json(new { success = false, errors = erros });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Usuário não autenticado." });

            try
            {
                // 🔹 Mantém o código original e atualiza apenas campos editáveis
                radio.CodRadio = radioExistente.CodRadio;
                radio.DataRegisto = radioExistente.DataRegisto;
                radio.UserId = user.Id;
                radio.DataUltimaAlterecao = DateTime.Now;

                _context.Entry(radio).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = $"Rádio atualizado com sucesso! Código: {radio.CodRadio}",
                    redirectUrl = Url.Action("Index")
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Radios.AnyAsync(e => e.Id == id))
                    return Json(new { success = false, message = "O rádio foi removido ou não existe mais." });

                return Json(new { success = false, message = "Erro de concorrência ao atualizar o registro." });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "Erro ao atualizar no banco de dados. Verifique os dados." });
            }
        }

        [HttpPost]
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec")]
        public IActionResult Delete(int id)
        {
            var radio = _context.Radios.Find(id);

            if (radio == null)
            {
                return Json(new { success = false, message = "Registo não encontrado." });
            }

            try
            {
                _context.Radios.Remove(radio);
                _context.SaveChanges();
                return Json(new
                {
                    success = true,
                    message = "Registo eliminado com sucesso!"
                });
            }
            catch (Exception ex)
            {
                // Loga o erro (por exemplo, em Serilog ou Console)
                return Json(new
                {
                    success = false,
                    message = $"Erro interno: {ex.Message}"
                });
            }
        }

    }
}
