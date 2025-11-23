using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class OrgaoUnidadesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public OrgaoUnidadesController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/OrgaoUnidades
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.OrgaoUnidades.Include(o => o.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/OrgaoUnidades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orgaoUnidade = await _context.OrgaoUnidades
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orgaoUnidade == null)
            {
                return NotFound();
            }

            return View(orgaoUnidade);
        }

        // GET: Admin/OrgaoUnidades/Create
        public IActionResult Create()
        {
            return PartialView("_Create", new OrgaoUnidade());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrgaoUnidade orgaoUnidade)
        {
            var userId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                orgaoUnidade.UserId = userId;
                _context.Add(orgaoUnidade);
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return PartialView("_Create", orgaoUnidade);
        }

        // GET: Admin/OrgaoUnidades/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false });
            }

            var orgaoUnidade = await _context.OrgaoUnidades.FindAsync(id);
            if (orgaoUnidade == null)
            {
                return Json(new { success = false });
            }
            return PartialView("_Edit", orgaoUnidade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrgaoUnidade orgaoUnidade)
        {
            if (id != orgaoUnidade.Id)
            {
                return Json(new { success = false });
            }

            var userId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                try
                {
                    orgaoUnidade.UserId = userId;
                    _context.Update(orgaoUnidade);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrgaoUnidadeExists(orgaoUnidade.Id))
                    {
                        return Json(new { success = false });
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return PartialView("_Edit", orgaoUnidade);
        }

        // GET: Municipios/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var orgaoUnidade = _context.OrgaoUnidades.Find(id);
            _context.OrgaoUnidades.Remove(orgaoUnidade);
            _context.SaveChanges();
            return Json(new { success = true });
        }
        private bool OrgaoUnidadeExists(int id)
        {
            return _context.OrgaoUnidades.Any(e => e.Id == id);
        }
    }
}
