using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SIG_PSPEP.Areas.Dpq.Controllers;

[Area("Dpq")]
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

    public IActionResult Privacy()
    {
        return View();
    }
}
