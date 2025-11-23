using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Mensagem([Bind("Id,Nome,Email,Telefone,Mensagem,DataRegisto,Estado")] MensagesAdmin mensagesAdmin)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(mensagesAdmin);
                    await _context.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        message = "Mensagem enviada com sucesso!"
                    });
                }

                // Se a validação do modelo falhar
                return Json(new
                {
                    success = false,
                    message = "Os campos são de carácter obrigatório.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
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

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UltimasMensagens()
        {
            var mensagens = await _context.MensagesAdmins
                .Where(m => m.Estado)
                .OrderByDescending(m => m.DataRegisto)
                .Take(6)
                .Select(m => new {
                    m.Id,
                    m.Nome,
                    m.Mensagem,
                    Data = m.DataRegisto.ToString("dd/MM/yyyy HH:mm")
                })
                .ToListAsync();

            return PartialView("_MensagensNotificacao", mensagens);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult TotalMensagens()
        {
            int total = _context.MensagesAdmins.Count(m => m.Estado == true);
            return Json(total);
        }

    }

}
