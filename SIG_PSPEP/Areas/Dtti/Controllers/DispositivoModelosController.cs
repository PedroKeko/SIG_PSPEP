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
    public class DispositivoModelosController : BaseController
    {
        private readonly ILogger<EfectividadesController> _logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public DispositivoModelosController(
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

        // GET: Dtti/DispositivoModelos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.DispositivoModelos.Include(d => d.DispositivoMarca).Include(d => d.User);
            return View(await appDbContext.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["DispositivoMarcaId"] = new SelectList(_context.DispositivoMarcas, "Id", "MarcasDispositivo");
            return PartialView("_Create", new DispositivoModelo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DispositivoModelo dispositivoModelo)
        {
            var userId = userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                // ⚙️ Validação personalizada: verificar se já existe registro com a mesma Marca + Modelo
                bool existe = await _context.DispositivoModelos
                    .AnyAsync(d => d.DispositivoMarcaId == dispositivoModelo.DispositivoMarcaId
                                && d.Modelo.ToLower() == dispositivoModelo.Modelo.ToLower());

                if (existe)
                {
                    // Retorna aviso ao JS → toastr.warning(result.message)
                    return Json(new { success = false, message = "Já existe um modelo com esta marca." });
                }

                dispositivoModelo.UserId = userId;
                _context.Add(dispositivoModelo);
                await _context.SaveChangesAsync();

                // Retorna sucesso → toastr.success()
                return Json(new { success = true });
            }

            // Caso ModelState inválido → reexibe formulário dentro do modal
            ViewData["DispositivoMarcaId"] = new SelectList(
                _context.DispositivoMarcas, "Id", "MarcasDispositivo", dispositivoModelo.DispositivoMarcaId
            );

            return PartialView("_Create", dispositivoModelo);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dispositivoModelo = await _context.DispositivoModelos.FindAsync(id);
            if (dispositivoModelo == null)
            {
                return NotFound();
            }

            ViewData["DispositivoMarcaId"] = new SelectList(
                _context.DispositivoMarcas, "Id", "MarcasDispositivo", dispositivoModelo.DispositivoMarcaId
            );

            return PartialView("_Edit", dispositivoModelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DispositivoModelo dispositivoModelo)
        {
            var userId = userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                // ⚙️ Validação personalizada: verificar se já existe outro registro com a mesma Marca + Modelo
                bool existe = await _context.DispositivoModelos
                    .AnyAsync(d =>
                        d.DispositivoMarcaId == dispositivoModelo.DispositivoMarcaId &&
                        d.Modelo.ToLower() == dispositivoModelo.Modelo.ToLower() &&
                        d.Id != dispositivoModelo.Id // Ignora o próprio registro atual
                    );

                if (existe)
                {
                    // Retorna aviso para o JavaScript → toastr.warning(result.message)
                    return Json(new { success = false, message = "Já existe um modelo com esta marca." });
                }

                try
                {
                    dispositivoModelo.UserId = userId;
                    _context.Update(dispositivoModelo);
                    await _context.SaveChangesAsync();

                    // Retorna sucesso → toastr.success()
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DispositivoModelos.Any(e => e.Id == dispositivoModelo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Caso ModelState inválido → reexibe formulário no modal
            ViewData["DispositivoMarcaId"] = new SelectList(
                _context.DispositivoMarcas, "Id", "MarcasDispositivo", dispositivoModelo.DispositivoMarcaId
            );

            return PartialView("_Edit", dispositivoModelo);
        }

        // POST: Dtti/RadioTipos/Delete/5
        [HttpPost]
        [Authorize(Roles = "Administrador, Chefe de Departamento")]
        public IActionResult Delete(int id)
        {
            var dispositivoModelos = _context.DispositivoModelos.Find(id);
            _context.DispositivoModelos.Remove(dispositivoModelos);
            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}
