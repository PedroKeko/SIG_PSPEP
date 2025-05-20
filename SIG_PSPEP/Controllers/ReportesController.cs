using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        public IActionResult Mensagem()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Mensagem(MensagesAdmin mensagem)
        {
            try
            {
                mensagem.Estado = true;
                mensagem.DataRegisto = DateTime.Now;
                _context.Add(mensagem);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Mensagem enviada com sucesso! O administrador entrará em contato."
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "Ocorreu um erro ao enviar a mensagem. Tente novamente."
                });
            }
        }


    }

}
