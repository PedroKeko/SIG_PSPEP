using Microsoft.AspNetCore.Authorization;
using System.Net.NetworkInformation;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;
using SIG_PSPEP.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace SIG_PSPEP.Controllers
{
    public class ContaController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly AppDbContext _context;
        private readonly IEmailSender _emailSender;

        public ContaController(AppDbContext context, 
            UserManager<IdentityUser> userManager,
         SignInManager<IdentityUser> signInManager,
         IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;
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

            // Registrar o log de acesso com sucesso
            var log = new LogsAcesso
            {
                UserId = user.Id,
                TipoAcesso = "Login",
                Obs = "Login efetuado com sucesso",
                DataRegisto = DateTime.Now
            };
            _context.LogsAcessos.Add(log);
            await _context.SaveChangesAsync();

            // Verificar se o usuário tem a role "Administrador"
            if (await userManager.IsInRoleAsync(user, "Administrador"))
            {
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Index", "Home", new { area = "Admin" })
                });
            }

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

            return PartialView("_PerfilUsuario", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Sair()
        {
            var user = await userManager.GetUserAsync(User);

            if (user != null)
            {
                // Obter IP (considerando X-Forwarded-For se estiver atrás de proxy)
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                }

                // Obter nome da máquina
                string hostName = "Desconhecido";
                try
                {
                    if (!string.IsNullOrEmpty(ipAddress) && ipAddress != "::1" && ipAddress != "127.0.0.1")
                    {
                        var hostEntry = await Dns.GetHostEntryAsync(ipAddress);
                        hostName = hostEntry.HostName;
                    }
                    else
                    {
                        hostName = Environment.MachineName; // Fallback para nome da máquina local
                    }
                }
                catch
                {
                    // Deixa "Desconhecido" se falhar
                }

                // MAC Address (apenas acessível no servidor, não do cliente via browser)
                string macAddress = "Indisponível";
                try
                {
                    var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                        .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                    var first = interfaces.FirstOrDefault();
                    if (first != null)
                    {
                        macAddress = string.Join(":", first.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")));
                    }
                }
                catch
                {
                    // Mantém "Indisponível" se der erro
                }

                var log = new LogsAcesso
                {
                    UserId = user.Id,
                    TipoAcesso = "Logout",
                    Obs = $"Logout efetuado com sucesso | IP: {ipAddress} | Host: {hostName} | MAC: {macAddress}",
                    DataRegisto = DateTime.Now
                };

                _context.LogsAcessos.Add(log);
                await _context.SaveChangesAsync();
            }

            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Conta");
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


        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
            {
                // Para segurança, não dizer que o email não existe
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action(nameof(ResetPassword), "Account", new { token, email = model.Email }, Request.Scheme);

            var mensagem = $"Por favor redefina sua senha clicando <a href='{resetLink}'>aqui</a>.";

            await _emailSender.SendEmailAsync(model.Email, "Redefinir senha", mensagem);

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
                return BadRequest("Token ou email inválido.");

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Não revelar que o usuário não existe
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
