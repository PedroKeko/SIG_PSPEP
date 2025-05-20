using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Controllers
{
    public class MensagesAdminController : Controller
    {
        private readonly AppDbContext _context;

        public MensagesAdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: MensagesAdmin
        public async Task<IActionResult> Index()
        {
            return View(await _context.MensagesAdmins.ToListAsync());
        }

        // GET: MensagesAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mensagesAdmin = await _context.MensagesAdmins
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mensagesAdmin == null)
            {
                return NotFound();
            }

            return View(mensagesAdmin);
        }

        // GET: MensagesAdmin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MensagesAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Email,Telefone,Mensagem,DataRegisto,Estado")] MensagesAdmin mensagesAdmin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mensagesAdmin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mensagesAdmin);
        }

        // GET: MensagesAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mensagesAdmin = await _context.MensagesAdmins.FindAsync(id);
            if (mensagesAdmin == null)
            {
                return NotFound();
            }
            return View(mensagesAdmin);
        }

        // POST: MensagesAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,Telefone,Mensagem,DataRegisto,Estado")] MensagesAdmin mensagesAdmin)
        {
            if (id != mensagesAdmin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mensagesAdmin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MensagesAdminExists(mensagesAdmin.Id))
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
            return View(mensagesAdmin);
        }

        // GET: MensagesAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mensagesAdmin = await _context.MensagesAdmins
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mensagesAdmin == null)
            {
                return NotFound();
            }

            return View(mensagesAdmin);
        }

        // POST: MensagesAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mensagesAdmin = await _context.MensagesAdmins.FindAsync(id);
            if (mensagesAdmin != null)
            {
                _context.MensagesAdmins.Remove(mensagesAdmin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MensagesAdminExists(int id)
        {
            return _context.MensagesAdmins.Any(e => e.Id == id);
        }
    }
}
