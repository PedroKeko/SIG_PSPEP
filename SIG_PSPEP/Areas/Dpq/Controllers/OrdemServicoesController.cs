using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Area("Dpq")]
    public class OrdemServicoesController : BaseController
    {
        private readonly ILogger<EfectividadesController> _logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public OrdemServicoesController(
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

        // GET: Dpq/OrdemServicoes
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec_Esp")]
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.OrdemServicos.Include(o => o.User);
            return View(await appDbContext.ToListAsync());
        }


        [Authorize(Policy = "Require_Admin_ChDepar_ChSec")]
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec")]
        public IActionResult Create(OrdemServico ordemServico)
        {
            var userId = userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                ordemServico.UserId = userId;
                _context.Add(ordemServico);
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return PartialView("_Create", ordemServico);
        }

        // GET: Dpq/OrdemServicoes/Edit/5
        [Authorize(Policy = "Require_Admin_ChDepar")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ordemServico = _context.OrdemServicos.Find(id);
            if (ordemServico == null)
            {
                return NotFound();
            }
            return PartialView("_Edit", ordemServico);
        }

        // POST: OrdemServicoes/Edit/5
        [HttpPost]
        [Authorize(Policy = "Require_Admin_ChDepar_ChSec")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrdemServico ordemServico)
        {
            var userId = userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                try
                {
                    ordemServico.UserId = userId;
                    _context.Update(ordemServico);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdemServicoExists(ordemServico.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return PartialView("_Edit", ordemServico);
        }

        // GET: Municipios/Delete/5
        [HttpPost]
        [Authorize(Policy = "Require_Admin_ChDepar")]
        public IActionResult Delete(int id)
        {
            var ordemServico = _context.OrdemServicos.Find(id);
            _context.OrdemServicos.Remove(ordemServico);
            _context.SaveChanges();
            return Json(new { success = true });
        }

        private bool OrdemServicoExists(int id)
        {
            return _context.OrdemServicos.Any(e => e.Id == id);
        }
    }
}
