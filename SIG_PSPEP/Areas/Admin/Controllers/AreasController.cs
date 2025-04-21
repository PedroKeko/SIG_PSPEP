using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class AreasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public AreasController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Areas
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Areas.Include(a => a.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/Areas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var area = await _context.Areas
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }

        // GET: Municipios/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var area = _context.Areas.Find(id);
            _context.Areas.Remove(area);
            _context.SaveChanges();
            return Json(new { success = true });
        }

        private bool AreaExists(int id)
        {
            return _context.Areas.Any(e => e.Id == id);
        }

        // GET: Areas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false });
            }

            var area = await _context.Areas.FindAsync(id);
            if (area == null)
            {
                return Json(new { success = false });
            }
            return PartialView("_Edit", area);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NomeArea,DescricaoArea,Id,Estado,DataUltimaAlterecao")] Area area)
        {
            if (id != area.Id)
            {
                return Json(new { success = false });
            }

            var userId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                try
                {
                    area.UserId = userId;
                    _context.Update(area);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AreaExists(area.Id))
                    {
                        return Json(new { success = false });
                    }
                    else
                    {
                        throw;
                    }
                }
                return Json(new { success = true });
            }

            return View("_Edit", area);
        }

    }
}
