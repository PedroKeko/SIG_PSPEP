using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SIG_PSPEP.Context;

namespace SIG_PSPEP.Areas.Dtti.Controllers;

[Area("Dtti")]
public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;

    public HomeController(
        AppDbContext context,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ILogger<HomeController> logger)
        : base(context)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        _logger = logger;
    }

    public IActionResult Index()
    {
        #region Segurança da Área
        //if (!UsuarioTemAcessoAAlgumaArea("DTTI", "ADMIN"))
        //{
        //    return Forbid();
        //}
        #endregion

        return View();
    }

}
