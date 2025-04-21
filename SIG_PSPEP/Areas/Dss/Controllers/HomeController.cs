using Microsoft.AspNetCore.Mvc;

namespace SIG_PSPEP.Areas.Dss.Controllers;

[Area("Dss")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult AcessoNegado()
    {
        return View();
    }
}
