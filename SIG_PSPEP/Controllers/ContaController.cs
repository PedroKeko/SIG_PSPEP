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
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home", "");
                }

                ModelState.AddModelError(string.Empty, "Senha ou Usuário Inválido");
            }

            return View(model);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PerfilUsuario()
        {
            // Obtém o usuário logado
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("Usuário não encontrado");
            }

            var userNameFoto = await _context.UsuarioAutes
                .FirstOrDefaultAsync(u => u.UserId == user.Id);

            // Obtém as roles do usuário
            var roles = await userManager.GetRolesAsync(user);

            var perfilViewModel = new PerfilUsuarioViewModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault()
            };

            return View(perfilViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AlterarSenha(PerfilUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Retorna ao perfil com os erros de validação
                return await RetornarPerfilComErros(model);
            }

            // Obtém o usuário logado
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            // Tenta alterar a senha do usuário
            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                // Faz logout após a alteração da senha
                await signInManager.SignOutAsync();
                TempData["Message"] = "Senha alterada com sucesso. Por favor, faça login novamente.";
                return RedirectToAction("Login", "Account", "");
            }

            // Adiciona erros ao ModelState se a alteração falhar
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // Retorna ao perfil com mensagens de erro
            return await RetornarPerfilComErros(model);
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

        [Authorize]
        [HttpGet]
        [Route("/Conta/AcessoNegado")]
        public ActionResult AcessoNegado()
        {
            return View();
        }
    }
}