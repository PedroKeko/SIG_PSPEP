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
    public class DispositivoMarcasController : BaseController
    {
        private readonly ILogger<EfectividadesController> _logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public DispositivoMarcasController(
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

        // GET: Dtti/DispositivoMarcas
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.DispositivoMarcas.Include(d => d.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Dtti/DispositivoTipos/Create
        public IActionResult Create()
        {
            return PartialView("_Create", new DispositivoMarca());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DispositivoMarca dispositivoMarca)
        {
            var userId = userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                // Verifica se já existe uma marca com o mesmo nome (case insensitive)
                bool marcaExiste = await _context.DispositivoMarcas
                    .AnyAsync(m => m.MarcasDispositivo.ToLower() == dispositivoMarca.MarcasDispositivo.ToLower());

                if (marcaExiste)
                {
                    // Retorna erro em JSON — pode ser tratado no JavaScript
                    return Json(new { success = false, message = "Já existe uma marca com este nome." });
                }

                dispositivoMarca.UserId = userId;
                _context.Add(dispositivoMarca);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }

            return PartialView("_Create", dispositivoMarca);
        }


        // GET: Editar
        public async Task<IActionResult> Edit(int id)
        {
            var dispositivoMarca = await _context.DispositivoMarcas.FindAsync(id);
            if (dispositivoMarca == null)
            {
                return NotFound();
            }

            return PartialView("_Edit", dispositivoMarca);
        }

        // POST: Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DispositivoMarca dispositivoMarca)
        {
            var userId = userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                // Verifica se já existe outra marca com o mesmo nome
                bool marcaExiste = await _context.DispositivoMarcas
                    .AnyAsync(m => m.MarcasDispositivo.ToLower() == dispositivoMarca.MarcasDispositivo.ToLower() && m.Id != dispositivoMarca.Id);

                if (marcaExiste)
                {
                    return Json(new { success = false, message = "Já existe uma marca com este nome." });
                }

                try
                {
                    dispositivoMarca.UserId = userId;
                    _context.Update(dispositivoMarca);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DispositivoMarcas.Any(e => e.Id == dispositivoMarca.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return PartialView("_Edit", dispositivoMarca);
        }

        // POST: Dtti/RadioTipos/Delete/5
        [HttpPost]
        [Authorize(Roles = "Administrador, Chefe de Departamento")]
        public IActionResult Delete(int id)
        {
            var dispositivoMarca = _context.DispositivoMarcas.Find(id);
            _context.DispositivoMarcas.Remove(dispositivoMarca);
            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}
