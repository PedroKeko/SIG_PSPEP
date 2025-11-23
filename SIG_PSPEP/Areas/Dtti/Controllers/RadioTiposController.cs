using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Areas.Dpq.Controllers;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidade;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Dtti.Controllers
{
    [Area("Dtti")]
    [Authorize(Policy = "Require_Admin_ChDepar_ChSec_Esp")]
    public class RadioTiposController : BaseController
    {
        private readonly ILogger<EfectividadesController> _logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public RadioTiposController(
        AppDbContext context,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ILogger<EfectividadesController> logger)
        : base(context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _logger = logger;
        }

        // GET: Dtti/RadioTipos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.RadioTipos.Include(r => r.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Dtti/RadioTipos/Create
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec_Esp")]
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Create", new RadioTipo());
        }

        [HttpPost]
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec_Esp")]
        public IActionResult Create(RadioTipo radioTipo)
        {
            var userId = userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                radioTipo.UserId = userId;
                _context.Add(radioTipo);
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return PartialView("_Create", radioTipo);
        }

        // GET: Dtti/RadioTipos/Edit/5
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var radioTipo = _context.RadioTipos.Find(id);
            if (radioTipo == null)
            {
                return NotFound();
            }
            return PartialView("_Edit", radioTipo);
        }

        // POST: Municipios/Edit/5
        [HttpPost]
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RadioTipo radioTipo)
        {
            var userId = userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                try
                {
                    radioTipo.UserId = userId;
                    _context.Update(radioTipo);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RadioTipoExists(radioTipo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return PartialView("_Edit", radioTipo);
        }

        // POST: Dtti/RadioTipos/Delete/5
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var radioTipo = _context.RadioTipos.Find(id);
            _context.RadioTipos.Remove(radioTipo);
            _context.SaveChanges();
            return Json(new { success = true });
        }

        private bool RadioTipoExists(int id)
        {
            return _context.RadioTipos.Any(e => e.Id == id);
        }
    }
}
