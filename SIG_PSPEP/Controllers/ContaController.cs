using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;
using SIG_PSPEP.Models;
using System.Net;
using System.Net.NetworkInformation;

namespace SIG_PSPEP.Controllers
{
    public class ContaController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly AppDbContext _context;
        private readonly IEmailSender _emailSender;

        public ContaController(
            AppDbContext context,
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RecuperarSenha()
        {
            return View("RecuperarSenha");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecuperarSenha(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(string.Empty, "O e-mail é obrigatório.");
                return View("RecuperarSenha");
            }

            var usuario = await userManager.FindByEmailAsync(email);
            if (usuario == null || !(await userManager.IsEmailConfirmedAsync(usuario)))
            {
                // Não revela se o usuário existe
                return Redirect("/conta/login");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(usuario);
            var link = Url.Action(nameof(RedefinirSenha), "Conta", new { token, email = usuario.Email }, Request.Scheme);

            string html = $@"
        <!DOCTYPE html>
        <html lang='pt'>
        <head>
          <meta charset='UTF-8' />
          <title>Redefinição de Senha</title>
          <style>
            body {{
              font-family: Arial, sans-serif;
              background-color: #f4f6f8;
              margin: 0;
              padding: 0;
            }}
            .email-container {{
              max-width: 600px;
              margin: auto;
              background-color: #ffffff;
              border: 1px solid #dcdcdc;
              border-radius: 8px;
              overflow: hidden;
            }}
            .header {{
              background-color: #00264d;
              color: white;
              padding: 20px;
              text-align: center;
            }}
            .header h1 {{
              margin: 0;
              font-size: 20px;
            }}
            .content {{
              padding: 30px 20px;
              color: #333;
            }}
            .content h2 {{
              color: #00264d;
            }}
            .btn {{
              display: inline-block;
              background-color: #ffc107;
              color: #00264d;
              padding: 10px 20px;
              text-decoration: none;
              border-radius: 5px;
              margin-top: 20px;
            }}
            .footer {{
              background-color: #f2f2f2;
              padding: 10px 20px;
              font-size: 12px;
              color: #555;
              text-align: center;
            }}
          </style>
        </head>
        <body>
          <div class='email-container'>
            <div class='header'>
              <h1>SIG PSPEP - Redefinição de Senha</h1>
            </div>
            <div class='content'>
              <h2>Olá, efetivo!</h2>
              <p>Recebemos uma solicitação para redefinir sua senha.</p>
              <p>Para continuar, clique no botão abaixo:</p>
              <a href='{link}' class='btn'>Redefinir Senha</a>
              <p>Se você não solicitou essa alteração, ignore este e-mail.</p>
            </div>
            <div class='footer'>
              Este e-mail foi enviado automaticamente pelo SIG-PSPEP<br>
              Departamento de Telecomunicações e Tecnologia de Informação<br>
              Polícia de Segurança Pessoal e de Entidades Protocolares<br>
              Polícia Nacional de Angola<br>
              Ministério do Interior
            </div>
          </div>
        </body>
        </html>";

            await _emailSender.SendEmailAsync(
                usuario.Email,
                "Redefinição de Senha",
                html
            );

            return Redirect("/conta/login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RedefinirSenha(string token, string email)
        {
            if (token == null || email == null)
                return RedirectToAction("Login");

            var modelo = new RedefinirSenhaViewModel { Token = token, Email = email };
            return View("RedefinirSenha", modelo);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RedefinirSenha(RedefinirSenhaViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View("RedefinirSenha", modelo);

            var usuario = await userManager.FindByEmailAsync(modelo.Email);
            if (usuario == null)
                return Redirect("/conta/login");

            var resultado = await userManager.ResetPasswordAsync(usuario, modelo.Token, modelo.NovaSenha);
            if (resultado.Succeeded)
                return Redirect("/conta/login");

            foreach (var erro in resultado.Errors)
                ModelState.AddModelError(string.Empty, erro.Description);

            return View("RedefinirSenha", modelo);
        }
    }
}