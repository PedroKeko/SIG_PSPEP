using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Areas.Dtti.Models;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Dtti.Controllers
{
    [Area("Dtti")]
    public class RadioGuiasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RadioGuiasController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Dtti/RadioGuias

        public async Task<IActionResult> Index()
        {
            var guias = await (
                from g in _context.RadioGuias

                    // Subconsulta: criador da guia
                let userAut = _context.UsuarioAutes
                    .Include(u => u.User)
                    .FirstOrDefault(u => u.UserId == g.UserId)

                // Subconsulta: chefe/aprovador
                let chefeAut = _context.UsuarioAutes
                    .Include(u => u.User)
                    .FirstOrDefault(u => u.UserId == g.ChefeId)

                orderby g.DataRegisto descending // 👈 ordena pela data mais recente

                select new RadioGuiaViewModel
                {
                    Id = g.Id,
                    NumGuia = g.NumGuia,
                    DataRegisto = g.DataRegisto,
                    Aprovado = g.Aprovado,
                    DataAprovacao = g.DataAprovacao,

                    NomeUsuario = userAut != null && userAut.User != null ? userAut.User.UserName : "N/A",
                    NomeChefe = chefeAut != null && chefeAut.User != null ? chefeAut.User.UserName : "N/A",

                    QuantidadeRadios = _context.RadioMovimentos
                        .Count(rm => rm.RadioGuiaId == g.Id && rm.TipoMovimento == "Saída")
                }
            ).ToListAsync();

            return View(guias);
        }


        // GET: Dtti/RadioGuias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var guia = await (
                from g in _context.RadioGuias
                where g.Id == id

                // Criador da guia
                join userAut in _context.UsuarioAutes
                    .Include(u => u.Efectivo)
                    .Include(u => u.User)
                on g.UserId equals userAut.UserId into criadorJoin
                from userAut in criadorJoin.DefaultIfEmpty()

                    // Chefe aprovador
                join chefeAut in _context.UsuarioAutes
                    .Include(u => u.Efectivo)
                    .Include(u => u.User)
                on g.ChefeId equals chefeAut.UserId into chefeJoin
                from chefeAut in chefeJoin.DefaultIfEmpty()

                select new RadioGuiaDetalhesViewModel
                {
                    Id = g.Id,
                    NumGuia = g.NumGuia,
                    Observacao = g.Observacao,
                    DataRegisto = g.DataRegisto,
                    Aprovado = g.Aprovado,
                    DataAprovacao = g.DataAprovacao,

                    NomeUsuario = userAut != null && userAut.User != null
                        ? userAut.User.UserName
                        : "N/A",

                    NomeChefe = chefeAut != null && chefeAut.User != null
                        ? chefeAut.User.UserName
                        : "N/A",

                    Radios = _context.RadioMovimentos
                        .Include(rm => rm.Radio)
                        .Include(rm => rm.OrgUnidPnaMinint)
                        .Where(rm => rm.RadioGuiaId == g.Id && rm.TipoMovimento == "Saída")
                        .Select(rm => new RadioItemViewModel
                        {
                            Id = rm.Radio.Id,
                            CodRadio = rm.Radio.CodRadio,
                            IdRadio = rm.Radio.IdRadio,
                            NumSerie = rm.Radio.NumSerie,
                            EstadoTecnico = rm.Radio.EstadoTecnico,
                            TipoMovimento = rm.TipoMovimento,
                            OrgUnidPnaMinint = rm.OrgUnidPnaMinint != null ? rm.OrgUnidPnaMinint.NomeOrgaoUnidade : "N/A"
                        }).ToList()
                }
            ).FirstOrDefaultAsync();

            if (guia == null)
                return NotFound();

            return PartialView("_Details", guia);
        }

        [HttpPost]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AprovarGuia(int idGuia)
        {
            var guia = await _context.RadioGuias.FindAsync(idGuia);
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (guia == null)
            {
                return Json(new { success = false, message = "Guia não encontrada." });
            }

            if (guia.Aprovado)
            {
                return Json(new { success = false, message = "Esta guia já foi aprovada anteriormente." });
            }

            guia.Aprovado = true;
            guia.ChefeId = userId;
            guia.DataAprovacao = DateTime.Now;

            _context.Update(guia);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Guia aprovada com sucesso!" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Busca a guia pelo ID
                var guia = await _context.RadioGuias.FindAsync(id);

                if (guia == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Guia não encontrada."
                    });
                }

                // Busca todos os movimentos relacionados à guia
                var movimentos = await _context.RadioMovimentos
                    .Where(rm => rm.RadioGuiaId == id)
                    .ToListAsync();

                if (movimentos.Any())
                {
                    _context.RadioMovimentos.RemoveRange(movimentos);
                }

                // Remove a guia após remover os movimentos
                _context.RadioGuias.Remove(guia);

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Guia e todos os movimentos associados foram eliminados com sucesso!"
                });
            }
            catch (Exception ex)
            {
                // Logar erro (opcional)
                Console.WriteLine($"Erro ao eliminar guia: {ex.Message}");

                return Json(new
                {
                    success = false,
                    message = "Ocorreu um erro inesperado ao eliminar o registo."
                });
            }
        }

    }
}
