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
    public class DispLocalizacaoController : BaseController
    {
        private readonly ILogger<EfectividadesController> _logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public DispLocalizacaoController(
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

        // GET: Dtti/DispLocalizacao
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.DispositivoLocalizacoes.Include(d => d.Dispositivo).Include(d => d.OrgUnidPnaMinint).Include(d => d.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Dtti/DispLocalizacao/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dispositivoLocalizacao = await _context.DispositivoLocalizacoes
                .Include(d => d.Dispositivo)
                .Include(d => d.OrgUnidPnaMinint)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dispositivoLocalizacao == null)
            {
                return NotFound();
            }

            return View(dispositivoLocalizacao);
        }

        // GET: Dtti/DispLocalizacao/Create
        public IActionResult Create(int id)
        {
            var dispositivo = _context.Dispositivos
                .Where(d => d.Id == id)
                .FirstOrDefault();

            if (dispositivo == null)
                return NotFound();

            ViewBag.EstadoTecnicoList = new SelectList(new List<string>
    {
        "Funcional",
        "Avariado",
        "Obsoleto",
        "Em Manutenção",
        "Em Reparação"
    });

            ViewData["OrgUnidPnaMinintId"] = new SelectList(
                _context.OrgUnidPnaMinints,
                "Id", "Sigla"
            );

            // devolve o modelo já com o dispositivo incluído
            return PartialView("_Create", new DispositivoLocalizacao
            {
                DispositivoId = dispositivo.Id,
                Dispositivo = dispositivo
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, DispositivoLocalizacao dispositivoLocalizacao)
        {
            var userId = userManager.GetUserId(User);

            var dispositivo = await _context.Dispositivos.FindAsync(dispositivoLocalizacao.DispositivoId);
            if (dispositivo == null)
                return Json(new { success = false, message = "Dispositivo não encontrado." });

            if (ModelState.IsValid)
            {
                dispositivoLocalizacao.UserId = userId;
                dispositivoLocalizacao.DispositivoId = id;
                _context.Add(dispositivoLocalizacao);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Localização cadastrada com sucesso!" });
            }

            // 🔹 Recarregar os SelectLists e o ViewData necessários
            ViewData["DispositivoId"] = new SelectList(
                _context.Dispositivos.Where(d => d.Id == dispositivoLocalizacao.DispositivoId),
                "Id", "Cod_Dispositivo",
                dispositivoLocalizacao.DispositivoId
            );

            ViewData["OrgUnidPnaMinintId"] = new SelectList(
                _context.OrgUnidPnaMinints, "Id", "Sigla", dispositivoLocalizacao.OrgUnidPnaMinintId
            );

            ViewBag.EstadoTecnicoList = new SelectList(new List<string>
    {
        "Funcional", "Avariado", "Obsoleto", "Em Manutenção", "Em Reparação"
    }, dispositivoLocalizacao.EstadoTecnico);

            return PartialView("_Create", dispositivoLocalizacao);
        }

        // GET: Dtti/DispLocalizacao/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dispositivoLocalizacao = await _context.DispositivoLocalizacoes.FindAsync(id);
            if (dispositivoLocalizacao == null)
            {
                return NotFound();
            }
            ViewData["DispositivoId"] = new SelectList(_context.Dispositivos, "Id", "Id", dispositivoLocalizacao.DispositivoId);
            ViewData["OrgUnidPnaMinintId"] = new SelectList(_context.OrgUnidPnaMinints, "Id", "Id", dispositivoLocalizacao.OrgUnidPnaMinintId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", dispositivoLocalizacao.UserId);
            return View(dispositivoLocalizacao);
        }

        // POST: Dtti/DispLocalizacao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DispositivoId,OrgUnidPnaMinintId,Obs,EstadoTecnico,Id,Estado,DataRegisto,DataUltimaAlterecao,UserId")] DispositivoLocalizacao dispositivoLocalizacao)
        {
            if (id != dispositivoLocalizacao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dispositivoLocalizacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DispositivoLocalizacaoExists(dispositivoLocalizacao.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DispositivoId"] = new SelectList(_context.Dispositivos, "Id", "Id", dispositivoLocalizacao.DispositivoId);
            ViewData["OrgUnidPnaMinintId"] = new SelectList(_context.OrgUnidPnaMinints, "Id", "Id", dispositivoLocalizacao.OrgUnidPnaMinintId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", dispositivoLocalizacao.UserId);
            return View(dispositivoLocalizacao);
        }

        // GET: Dtti/DispLocalizacao/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dispositivoLocalizacao = await _context.DispositivoLocalizacoes
                .Include(d => d.Dispositivo)
                .Include(d => d.OrgUnidPnaMinint)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dispositivoLocalizacao == null)
            {
                return NotFound();
            }

            return View(dispositivoLocalizacao);
        }

        // POST: Dtti/DispLocalizacao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dispositivoLocalizacao = await _context.DispositivoLocalizacoes.FindAsync(id);
            if (dispositivoLocalizacao != null)
            {
                _context.DispositivoLocalizacoes.Remove(dispositivoLocalizacao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DispositivoLocalizacaoExists(int id)
        {
            return _context.DispositivoLocalizacoes.Any(e => e.Id == id);
        }
    }
}
