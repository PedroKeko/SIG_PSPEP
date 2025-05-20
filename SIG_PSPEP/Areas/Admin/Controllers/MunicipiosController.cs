using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidade;
using SIG_PSPEP.Entidades;
using SIG_PSPEP.Services;

namespace SIG_PSPEP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class MunicipiosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly LogsEventosService _logsEventosService;

        public MunicipiosController(AppDbContext context, UserManager<IdentityUser> userManager, LogsEventosService logsEventosService)
        {
            _context = context;
            _userManager = userManager;
            _logsEventosService = logsEventosService;
        }

        // GET: Municipios
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Municipios.Include(m => m.Provincia).Include(m => m.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Municipios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var municipio = await _context.Municipios
                .Include(m => m.Provincia)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (municipio == null)
            {
                return NotFound();
            }

            return View(municipio);
        }

        // GET: Municipios/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["ProvinciaId"] = new SelectList(_context.Provincias, "Id", "Nome");
            return PartialView("_Create", new Municipio());
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Municipio municipio)
        {
            var userId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                municipio.UserId = userId;
                _context.Add(municipio);
                _context.SaveChanges();
                await _logsEventosService.LogEventAsync(userId, "Insersão", "Inseriu Municipio do "+ municipio.Nome +" !");
                return Json(new { success = true });
            }

            return PartialView("_Create", municipio);
        }

        // GET: Municipios/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
                var municipio = _context.Municipios.Find(id);
            if (municipio == null)
            {
                return NotFound();
            }
            ViewData["ProvinciaId"] = new SelectList(_context.Provincias, "Id", "Nome", municipio.ProvinciaId);
            return PartialView("_Edit", municipio);
        }

        // POST: Municipios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Municipio municipio)
        {
            var userId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                try
                { 
                    municipio.UserId = userId;
                    _context.Update(municipio);
                    await _context.SaveChangesAsync();
                    await _logsEventosService.LogEventAsync(userId, "Alteração", "Alterou Municipio do " + municipio.Nome + " !");
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MunicipioExists(municipio.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return PartialView("_Edit", municipio);
        }
        private bool MunicipioExists(int id)
        {
            return _context.Municipios.Any(e => e.Id == id);
        }


        // GET: Municipios/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var userId = _userManager.GetUserId(User);
            var municipio = _context.Municipios.Find(id);
            _context.Municipios.Remove(municipio);
            _context.SaveChanges();
            await _logsEventosService.LogEventAsync(userId, "Exclusão", "Excluíu Municipio do " + municipio.Nome + " !");
            return Json(new { success = true });
        }
    }
}
