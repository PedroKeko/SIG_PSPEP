using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class MensagesAdminController : Controller
    {
        private readonly AppDbContext _context;

        public MensagesAdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: MensagesAdmin Todas
        public async Task<IActionResult> Index()
        {
            return View(await _context.MensagesAdmins.ToListAsync());
        }

        // GET: Mensages Nao Lidas
        public async Task<IActionResult> Naolidas()
        {
            var mensagensNaoLidas = await _context.MensagesAdmins
                .Where(m => m.Estado == true)
                .OrderByDescending(m => m.DataRegisto)
                .ToListAsync();

            return View(mensagensNaoLidas);
        }
        public async Task<IActionResult> lidas()
        {
            var mensagensNaoLidas = await _context.MensagesAdmins
                .Where(m => m.Estado == false)
                .OrderByDescending(m => m.DataRegisto)
                .ToListAsync();

            return View(mensagensNaoLidas);
        }

        // GET: MensagesAdmin/Details/5
        public async Task<IActionResult> DetalhesMensagem(int? id)
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

            // Se a mensagem ainda não foi lida (Estado = true), marca como lida (Estado = false)
            if (mensagesAdmin.Estado)
            {
                mensagesAdmin.Estado = false;
                _context.Update(mensagesAdmin);
                await _context.SaveChangesAsync();
            }

            return View(mensagesAdmin);
        }

        // GET: MensagesAdmin/Details/5
        public async Task<IActionResult> DetalhesMensagemPartial(int? id)
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

            // Se a mensagem ainda não foi lida (Estado = true), marca como lida (Estado = false)
            if (mensagesAdmin.Estado)
            {
                mensagesAdmin.Estado = false;
                _context.Update(mensagesAdmin);
                await _context.SaveChangesAsync();
            }

            return PartialView("_DetalhesMensagem", mensagesAdmin);
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
