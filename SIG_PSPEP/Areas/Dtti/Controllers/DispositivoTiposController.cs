using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Areas.Dpq.Controllers;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIG_PSPEP.Areas.Dtti.Controllers
{
    [Area("Dtti")]
    [Authorize(Policy = "Require_Admin_ChDepar_ChSec_Esp")]
    public class DispositivoTiposController : BaseController
    {
        private readonly ILogger<EfectividadesController> _logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public DispositivoTiposController(
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

        // GET: Dtti/DispositivoTipos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.DispositivoTipos.Include(d => d.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Dtti/DispositivoTipos/Create
        public IActionResult Create()
        {
            return PartialView("_Create", new DispositivoTipo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DispositivoTipo dispositivoTipo)
        {
            var userId = userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                // Verifica se já existe uma marca com o mesmo nome (case insensitive)
                bool marcaExiste = await _context.DispositivoTipos
                    .AnyAsync(m => m.TiposDispositivo.ToLower() == dispositivoTipo.TiposDispositivo.ToLower());

                if (marcaExiste)
                {
                    // Retorna erro em JSON — pode ser tratado no JavaScript
                    return Json(new { success = false, message = "Já existe um tipo de Dispositivo com este nome." });
                }
                dispositivoTipo.UserId = userId;
                _context.Add(dispositivoTipo);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("_Create",dispositivoTipo);
        }

        public IActionResult Edit(int id)
        {
            var dispositivoTipo = _context.DispositivoTipos.Find(id);
            if (dispositivoTipo == null)
            {
                return NotFound();
            }
            return PartialView("_Edit", dispositivoTipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DispositivoTipo dispositivoTipo)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Edit", dispositivoTipo);
            }
            // Verifica se já existe outra marca com o mesmo nome
            bool marcaExiste = await _context.DispositivoTipos
                .AnyAsync(m => m.TiposDispositivo.ToLower() == dispositivoTipo.TiposDispositivo.ToLower() && m.Id != dispositivoTipo.Id);

            if (marcaExiste)
            {
                return Json(new { success = false, message = "Já existe uma marca com este nome." });
            }

            try
            {
                var userId = userManager.GetUserId(User);
                dispositivoTipo.UserId = userId;

                _context.Update(dispositivoTipo);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.DispositivoTipos.Any(e => e.Id == dispositivoTipo.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: Dtti/RadioTipos/Delete/5
        [HttpPost]
        [Authorize(Roles = "Administrador, Chefe de Departamento")]
        public IActionResult Delete(int id)
        {
            var dispositivoTipos = _context.DispositivoTipos.Find(id);
            _context.DispositivoTipos.Remove(dispositivoTipos);
            _context.SaveChanges();
            return Json(new { success = true });
        }

    }
}
