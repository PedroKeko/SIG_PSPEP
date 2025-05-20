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
    public class LogsEventosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly LogsEventosService _logsEventosService;

        public LogsEventosController(AppDbContext context, UserManager<IdentityUser> userManager, LogsEventosService logsEventosService)
        {
            _context = context;
            _userManager = userManager;
            _logsEventosService = logsEventosService;
        }

        // GET: Admin/LogsEventos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.LogsEventos.Include(l => l.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/LogsEventos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logsEvento = await _context.LogsEventos
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (logsEvento == null)
            {
                return NotFound();
            }

            return PartialView("_Detalhes", logsEvento);
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            var logsEventos = _context.LogsEventos.Find(id);
            _context.LogsEventos.Remove(logsEventos);
            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}
