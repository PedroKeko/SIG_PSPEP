using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Controllers
{
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

        // GET: Areas
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Areas.Include(a => a.User);
            return View(await appDbContext.ToListAsync());
        }

        public IActionResult Create()
        {
            return PartialView("_Create", new Area());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Area area)
        {
            var userId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                area.UserId = userId;
                _context.Add(area);
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return PartialView("_Create", area);
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
            return PartialView("_Edit",area);
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

        private bool AreaExists(int id)
        {
            return _context.Areas.Any(e => e.Id == id);
        }
    }
}
