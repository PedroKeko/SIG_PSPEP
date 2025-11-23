using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Areas.Admin.Models;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;
using SIG_PSPEP.Services;

namespace SIG_PSPEP.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrador")]
    public class HomeController(
        IWebHostEnvironment _hostingEnvironment,
        AppDbContext context,
        UserManager<IdentityUser> _userManager,
        RoleManager<IdentityRole> _roleManager,
        ImageCompressionService _imageCompressionService,
        SignInManager<IdentityUser> signInManager,
        ILogger<HomeController> logger // Corrigido aqui o nome
    ) : BaseController(context)
    {

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
        var orgaoUnidades = await _context.OrgaoUnidades.CountAsync();
        var provincias = await _context.Provincias.CountAsync();
        var municipios = await _context.Municipios.CountAsync();
        var logsEventos = await _context.LogsEventos.CountAsync();
        var logsAcessos = await _context.LogsAcessos.CountAsync();
        var bancos = await _context.Bancos.CountAsync();

        var usuariosAtivos = await _context.Users.Where(u => u.LockoutEnabled == true).CountAsync();
        var usuariosDesativados = await _context.Users.Where(u => u.LockoutEnabled == false).CountAsync();

        //Ultimos 5 usuarios
        var ultimosUsuarios = await _context.UsuarioAutes
            .Include(ua => ua.User)
            .OrderByDescending(ua => ua.Id)
            .Take(5)
            .Select(ua => ua.User.UserName)
            .ToListAsync();

        var UltimosLogins = await _context.LogsAcessos
       .Include(l => l.User) // Acesso ao email ou username
       .Where(l => l.TipoAcesso == "Login") // Filtra apenas logins
       .OrderByDescending(l => l.DataRegisto)
       .Take(5)
       .Select(l => new LogsAcessoViewModel
       {
           Email = l.User.UserName, // ou l.User.Email
           TipoAcesso = l.TipoAcesso,
           Obs = l.Obs,
           DataRegisto = l.DataRegisto
       })
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
            UltimosLogins = UltimosLogins,
            RegistrosPorMes = registrosPorMes,
            Provincias = provincias,
            Municipios = municipios,
            OrgaoUnidades = orgaoUnidades,
            LogsAcessos = logsAcessos,
            LogsEventos = logsEventos,
            Bancos = bancos,
        };

        return Json(resultado);
    }



}

