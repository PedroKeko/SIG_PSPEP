using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Models;

namespace SIG_PSPEP.Controllers
{
    public class ContaController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly AppDbContext _context;

        public ContaController(AppDbContext context, UserManager<IdentityUser> userManager,
         SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Dados inválidos. Verifique os campos e tente novamente."
                });
            }

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Senha ou Usuário Inválido"
                });
            }

            if (!user.LockoutEnabled)
            {
                return Json(new
                {
                    success = false,
                    message = "Sua conta está desativada. Contacte o administrador."
                });
            }

            var result = await signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, false);

            if (!result.Succeeded)
            {
                return Json(new
                {
                    success = false,
                    message = "Senha ou Usuário Inválido"
                });
            }

            // Verificar se o usuário tem a role "Administrador"
            if (await userManager.IsInRoleAsync(user, "Administrador"))
            {
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Index", "Home", new { area = "Admin" })
                });
            }

            // Senão, pega a área do UsuarioAute
            var usuarioAute = await _context.UsuarioAutes
                .Include(u => u.Area)
                .FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (usuarioAute == null || usuarioAute.Area == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Área de acesso não atribuída. Contacte o administrador."
                });
            }

            var area = usuarioAute.Area.NomeArea?.ToLower();

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("Index", "Home", new { area = area })
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PerfilUsuario()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Usuário não encontrado");
            }

            var usuarioAute = await _context.UsuarioAutes
                .Include(u => u.Efectivo)
                    .ThenInclude(e => e.FuncaoCargo)
                .Include(u => u.Efectivo.Patente)
                .Include(u => u.Efectivo.OrgaoUnidade)
                .Include(u => u.Area)
                .FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (usuarioAute == null)
            {
                return NotFound("Associação do usuário não encontrada");
            }

            var foto = await _context.FotoEfectivos
                .Where(f => f.EfectivoId == usuarioAute.EfectivoId)
                .Select(f => f.Foto)
                .FirstOrDefaultAsync();

            var roles = await userManager.GetRolesAsync(user);
            var claims = await userManager.GetClaimsAsync(user);

            var model = new PerfilUsuarioViewModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                NomeCompleto = usuarioAute.Efectivo?.NomeCompleto,
                Role = roles.FirstOrDefault(),
                Foto = foto,
                Roles = roles.ToList(),
                Claims = claims.Select(c => $"{c.Type}: {c.Value}").ToList()
            };

            ViewBag.Area = usuarioAute.Area;
            ViewBag.Efectivo = usuarioAute.Efectivo;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarSenha(PerfilUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, errors = erros });
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, errors = new[] { "Usuário não encontrado." } });
            }

            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                var erros = result.Errors.Select(e => e.Description).ToList();
                return Json(new { success = false, errors = erros });
            }

            return Json(new { success = true, message = "Senha alterada com sucesso!" });
        }

        // Método auxiliar para retornar a view de perfil com erros
        private async Task<IActionResult> RetornarPerfilComErros(PerfilUsuarioViewModel model)
        {
            // Obtém o usuário logado novamente para repopular os dados do perfil
            var user = await userManager.GetUserAsync(User);
            var userNameFoto = await _context.UsuarioAutes.FirstOrDefaultAsync(u => u.UserId == user.Id);

            // Obtém as roles do usuário
            var roles = await userManager.GetRolesAsync(user);

            // Atualiza o ViewModel com os dados corretos
            var perfilViewModel = new PerfilUsuarioViewModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault(),
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword,
                ConfirmNewPassword = model.ConfirmNewPassword
            };

            // Retorna a view com o perfil do usuário
            return View("PerfilUsuario", perfilViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Sair()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Conta", "");
        }

       
        [HttpGet]
        public async Task<IActionResult> RedirecParaArea()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (await userManager.IsInRoleAsync(user, "Administrador"))
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            var usuarioAute = await _context.UsuarioAutes
                .Include(u => u.Area)
                .FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (usuarioAute == null || usuarioAute.Area == null)
            {
                TempData["ErrorMessage"] = "Área de acesso não atribuída. Contacte o administrador.";
                return RedirectToAction("Login", "Account");
            }

            var area = usuarioAute.Area.NomeArea?.ToLower();
            var currentArea = RouteData.Values["area"]?.ToString()?.ToLower();

            if (currentArea != area)
            {
                return RedirectToAction("Index", "Home", new { area = area });
            }

            return RedirectToAction("Index", "Home", new { area = currentArea });
        }


        [Authorize]
        [HttpGet]
        [Route("/Conta/AcessoNegado")]
        public ActionResult AcessoNegado()
        {
            return View();
        }
    }
}