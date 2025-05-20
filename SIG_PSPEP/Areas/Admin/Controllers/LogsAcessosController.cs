using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;
using SIG_PSPEP.Services;

namespace SIG_PSPEP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class LogsAcessosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly LogsEventosService _logsEventosService;

        public LogsAcessosController(AppDbContext context, UserManager<IdentityUser> userManager, LogsEventosService logsEventosService)
        {
            _context = context;
            _userManager = userManager;
            _logsEventosService = logsEventosService;
        }

        // GET: Admin/LogsAcessos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.LogsAcessos.Include(l => l.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/LogsAcessos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logsAcesso = await _context.LogsAcessos
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (logsAcesso == null)
            {
                return NotFound();
            }

            return PartialView("_Detalhes", logsAcesso);
        }

     
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var logsAcessos = _context.LogsAcessos.Find(id);
            _context.LogsAcessos.Remove(logsAcessos);
            _context.SaveChanges();
            await _logsEventosService.LogEventAsync(userId, "Exclusão", "Excluíu Log de Acesso do tipo " + logsAcessos.TipoAcesso + " !");
            return Json(new { success = true });
        }

    }
}
