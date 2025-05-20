using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidade;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Dpq.Controllers
{
    [Area("DPQ")]
    public class MunicipiosController : BaseController
    {
        private readonly ILogger<EfectividadesController> _logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public MunicipiosController(
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

        // GET: Municipios
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec_Esp")]
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Municipios.Include(m => m.Provincia).Include(m => m.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Municipios/Create
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec_Esp")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["ProvinciaId"] = new SelectList(_context.Provincias, "Id", "Nome");
            return PartialView("_Create", new Municipio());
        }

        [HttpPost]
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec_Esp")]
        public IActionResult Create(Municipio municipio)
        {
            var userId = userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                municipio.UserId = userId;
                _context.Add(municipio);
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return PartialView("_Create", municipio);
        }

        // GET: Municipios/Edit/5
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec")]
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
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Municipio municipio)
        {
            var userId = userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                try
                { 
                    municipio.UserId = userId;
                    _context.Update(municipio);
                    await _context.SaveChangesAsync();
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
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var municipio = _context.Municipios.Find(id);
            _context.Municipios.Remove(municipio);
            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}
