using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Areas.Admin.Models;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrador")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly AppDbContext _context; 

    public HomeController(
        ILogger<HomeController> logger,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<IdentityUser> signInManager,
        AppDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ObterEstatisticasDashboard()
    {
        var totalUsuarios = await _userManager.Users.CountAsync();
        var totalRoles = await _roleManager.Roles.CountAsync();
        var totalAreas = await _context.Areas.CountAsync();
        var totalClaims = await _context.UserClaims.CountAsync();

        var usuariosAtivos = await _context.Users.Where(u => u.LockoutEnabled == false).CountAsync();
        var usuariosDesativados = await _context.Users.Where(u => u.LockoutEnabled == true).CountAsync();

        var ultimosUsuarios = await _context.Users
            .OrderByDescending(u => u.Id)
            .Take(5)
            .Select(u => u.UserName)
            .ToListAsync();

        var ultimosLogins = await _context.UserLogins
            .OrderByDescending(l => l.ProviderKey)
            .Take(5)
            .Select(l => l.ProviderDisplayName)
            .ToListAsync();

        // Agrupar por mês e contar os registros de usuários por mês
        var agrupamento = await _context.UsuarioAutes
            .Where(ua => ua.UserId != null)
            .Join(_context.Efectivos,
                ua => EF.Property<int?>(ua, "EfectivoId"),
                ef => ef.Id,
                (ua, ef) => new { Usuario = ua, Efectivo = ef })
            .GroupBy(x => new { x.Efectivo.DataRegisto.Year, x.Efectivo.DataRegisto.Month })
            .Select(group => new
            {
                Ano = group.Key.Year,
                Mes = group.Key.Month,
                Contagem = group.Count()
            })
            .ToListAsync(); // Executa a query no banco

        // Formata o resultado em memória
        var registrosPorMes = agrupamento
            .Select(g => new RegistroPorMes
            {
                AnoMes = $"{g.Mes:D2}/{g.Ano}", // Formata como MM/AAAA
                Contagem = g.Contagem
            })
            .OrderBy(x => x.AnoMes)
            .ToList();

        var resultado = new DashboardEstatisticas
        {
            TotalUsuarios = totalUsuarios,
            TotalRoles = totalRoles,
            TotalAreas = totalAreas,
            TotalClaims = totalClaims,
            UsuariosAtivos = usuariosAtivos,
            UsuariosDesativados = usuariosDesativados,
            UltimosUsuarios = ultimosUsuarios,
            UltimosLogins = ultimosLogins,
            RegistrosPorMes = registrosPorMes
        };

        return Json(resultado);
    }



}

