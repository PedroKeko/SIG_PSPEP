using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIG_PSPEP.Areas.Dtti.Controllers
{
    [Area("Dtti")]
    public class OrgUnidPnaMinintController : Controller
    {
        private readonly AppDbContext _context;

        public OrgUnidPnaMinintController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Dtti/OrgUnidPnaMinint
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.OrgUnidPnaMinints.Include(o => o.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Dtti/OrgUnidPnaMinint/Create
        public IActionResult Create()
        {
            return PartialView("_Create", new OrgUnidPnaMinint());
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrgUnidPnaMinint orgUnidPnaMinint)
        {
            // Validação de campos obrigatórios
            if (string.IsNullOrWhiteSpace(orgUnidPnaMinint.NomeOrgaoUnidade) ||
                string.IsNullOrWhiteSpace(orgUnidPnaMinint.Sigla))
            {
                ModelState.AddModelError(string.Empty, "Os campos 'Nome do Órgão/Unidade' e 'Sigla' são obrigatórios.");
            }

            // Validação de duplicidade
            bool nomeDuplicado = await _context.OrgUnidPnaMinints
                .AnyAsync(o => o.NomeOrgaoUnidade == orgUnidPnaMinint.NomeOrgaoUnidade);
            bool siglaDuplicada = await _context.OrgUnidPnaMinints
                .AnyAsync(o => o.Sigla == orgUnidPnaMinint.Sigla);

            if (nomeDuplicado)
            {
                ModelState.AddModelError("NomeOrgaoUnidade", "Já existe um órgão/unidade com este nome.");
            }

            if (siglaDuplicada)
            {
                ModelState.AddModelError("Sigla", "Já existe um órgão/unidade com esta sigla.");
            }

            if (ModelState.IsValid)
            {
                // Preenchendo campos automaticamente
                orgUnidPnaMinint.Estado = true;
                orgUnidPnaMinint.DataRegisto = DateTime.Now;
                orgUnidPnaMinint.DataUltimaAlterecao = DateTime.Now;
                orgUnidPnaMinint.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;

                _context.Add(orgUnidPnaMinint);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("_Create", orgUnidPnaMinint);
        }


        // GET: Dtti/OrgUnidPnaMinint/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var orgUnidPnaMinint = await _context.OrgUnidPnaMinints.FindAsync(id);
            if (orgUnidPnaMinint == null) return NotFound();

            return PartialView("_Edit", orgUnidPnaMinint);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrgUnidPnaMinint orgUnidPnaMinint)
        {
            if (id != orgUnidPnaMinint.Id) return NotFound();

            // Validação de campos obrigatórios
            if (string.IsNullOrWhiteSpace(orgUnidPnaMinint.NomeOrgaoUnidade) ||
                string.IsNullOrWhiteSpace(orgUnidPnaMinint.Sigla))
            {
                ModelState.AddModelError(string.Empty, "Os campos 'Nome do Órgão/Unidade' e 'Sigla' são obrigatórios.");
            }

            // Validação de duplicidade, ignorando o próprio registro
            bool nomeDuplicado = await _context.OrgUnidPnaMinints
                .AnyAsync(o => o.NomeOrgaoUnidade == orgUnidPnaMinint.NomeOrgaoUnidade && o.Id != orgUnidPnaMinint.Id);
            bool siglaDuplicada = await _context.OrgUnidPnaMinints
                .AnyAsync(o => o.Sigla == orgUnidPnaMinint.Sigla && o.Id != orgUnidPnaMinint.Id);

            if (nomeDuplicado)
                ModelState.AddModelError("NomeOrgaoUnidade", "Já existe um órgão/unidade com este nome.");

            if (siglaDuplicada)
                ModelState.AddModelError("Sigla", "Já existe um órgão/unidade com esta sigla.");

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualiza campos automaticamente
                    orgUnidPnaMinint.DataUltimaAlterecao = DateTime.Now;
                    orgUnidPnaMinint.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;

                    _context.Update(orgUnidPnaMinint);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrgUnidPnaMinintExists(orgUnidPnaMinint.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            return PartialView("_Edit", orgUnidPnaMinint);
        }

        // Método auxiliar para verificar existência
        private bool OrgUnidPnaMinintExists(int id)
        {
            return _context.OrgUnidPnaMinints.Any(e => e.Id == id);
        }


        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var orgUnidPnaMinints = _context.OrgUnidPnaMinints.Find(id);
            _context.OrgUnidPnaMinints.Remove(orgUnidPnaMinints);
            _context.SaveChanges();
            return Json(new { success = true });
        }

    }
}
