using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Areas.Dpq.Controllers;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Dtti.Controllers
{
    [Area("Dtti")]
    [Authorize(Policy = "Require_Admin_ChDepar_ChSec_Esp")]
    public class DispositivosController : BaseController
    {
        private readonly ILogger<EfectividadesController> _logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public DispositivosController(
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

        // GET: Dtti/Dispositivos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Dispositivos.Include(d => d.DispositivoMarca).Include(d => d.DispositivoModelo).Include(d => d.DispositivoTipo).Include(d => d.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Dtti/Dispositivos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dispositivo = await _context.Dispositivos
                .Include(d => d.DispositivoMarca)
                .Include(d => d.DispositivoModelo)
                .Include(d => d.DispositivoTipo)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dispositivo == null)
            {
                return NotFound();
            }

            return View(dispositivo);
        }

        // GET: Dtti/Dispositivos/Create
        public IActionResult Create()
        {
            ViewData["DispositivoMarcaId"] = new SelectList(_context.DispositivoMarcas, "Id", "MarcasDispositivo");
            ViewData["DispositivoModeloId"] = new SelectList(Enumerable.Empty<SelectListItem>()); // inicialmente vazio
            ViewData["DispositivoTipoId"] = new SelectList(_context.DispositivoTipos, "Id", "TiposDispositivo");
            return PartialView("_Create", new Dispositivo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dispositivo dispositivo)
        {
            var userId = userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                // 🧩 Validação do MAC (se informado)
                if (!string.IsNullOrWhiteSpace(dispositivo.MAC))
                {
                    bool macExiste = await _context.Dispositivos
                        .AnyAsync(d => d.MAC.ToLower() == dispositivo.MAC.ToLower());

                    if (macExiste)
                    {
                        // Retorna mensagem de aviso — toastr.warning no JS
                        return Json(new { success = false, message = "O endereço MAC já está registrado em outro dispositivo." });
                    }
                }

                // 🔢 Geração de código único e sequencial
                dispositivo.Cod_Dispositivo = await GerarCodigoUnicoAsync();
                dispositivo.UserId = userId;
                _context.Add(dispositivo);
                await _context.SaveChangesAsync();

                // Retorna sucesso — toastr.success
                return Json(new { success = true });
            }

            ViewData["DispositivoMarcaId"] = new SelectList(_context.DispositivoMarcas, "Id", "MarcasDispositivo", dispositivo.DispositivoMarcaId);
            ViewData["DispositivoModeloId"] = new SelectList(_context.DispositivoModelos, "Id", "Modelo", dispositivo.DispositivoModeloId);
            ViewData["DispositivoTipoId"] = new SelectList(_context.DispositivoTipos, "Id", "TiposDispositivo", dispositivo.DispositivoTipoId);

            return PartialView("_Create", dispositivo);
        }

        private async Task<string> GerarCodigoUnicoAsync()
        {
            // Prefixo base
            string prefixo = "DSP";

            // Busca o último código gerado no banco
            var ultimo = await _context.Dispositivos
                .OrderByDescending(d => d.Id)
                .Select(d => d.Cod_Dispositivo)
                .FirstOrDefaultAsync();

            int novoNumero = 1;

            if (!string.IsNullOrEmpty(ultimo))
            {
                // Extrai a parte numérica (ex: DSP-000123 → 123)
                var partes = ultimo.Split('-');
                if (partes.Length == 2 && int.TryParse(partes[1], out int numero))
                    novoNumero = numero + 1;
            }

            // Monta o novo código com 6 dígitos
            string novoCodigo = $"{prefixo}-{novoNumero.ToString("D6")}";

            // Garante que é único (em caso de inserções simultâneas)
            bool existe = await _context.Dispositivos.AnyAsync(d => d.Cod_Dispositivo == novoCodigo);
            while (existe)
            {
                novoNumero++;
                novoCodigo = $"{prefixo}-{novoNumero.ToString("D6")}";
                existe = await _context.Dispositivos.AnyAsync(d => d.Cod_Dispositivo == novoCodigo);
            }

            return novoCodigo;
        }

        [HttpGet]
        public async Task<IActionResult> GetModelosPorMarca(int marcaId)
        {
            var modelos = await _context.DispositivoModelos
                .Where(m => m.DispositivoMarcaId == marcaId)
                .Select(m => new { m.Id, m.Modelo })
                .ToListAsync();

            return Json(modelos);
        }


        // GET: Dtti/Dispositivos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var dispositivo = await _context.Dispositivos.FindAsync(id);
            if (dispositivo == null) return NotFound();

            ViewData["DispositivoMarcaId"] = new SelectList(_context.DispositivoMarcas, "Id", "MarcasDispositivo", dispositivo.DispositivoMarcaId);
            ViewData["DispositivoModeloId"] = new SelectList(_context.DispositivoModelos, "Id", "Modelo", dispositivo.DispositivoModeloId);
            ViewData["DispositivoTipoId"] = new SelectList(_context.DispositivoTipos, "Id", "TiposDispositivo", dispositivo.DispositivoTipoId);

            return PartialView("_Edit", dispositivo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Dispositivo dispositivo)
        {
            if (id != dispositivo.Id) return NotFound();

            var userId = userManager.GetUserId(User);

            // Busca o registro original
            var dispositivoOriginal = await _context.Dispositivos
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dispositivoOriginal == null) return NotFound();

            if (ModelState.IsValid)
            {
                // 🔹 Validação do MAC (ignora o próprio registro)
                if (!string.IsNullOrWhiteSpace(dispositivo.MAC))
                {
                    bool macExiste = await _context.Dispositivos
                        .AnyAsync(d => d.MAC.ToLower() == dispositivo.MAC.ToLower() && d.Id != dispositivo.Id);

                    if (macExiste)
                    {
                        return Json(new { success = false, message = "O endereço MAC já está registrado em outro dispositivo." });
                    }
                }

                try
                {
                    // Copia apenas os campos editáveis
                    dispositivoOriginal.DispositivoMarcaId = dispositivo.DispositivoMarcaId;
                    dispositivoOriginal.DispositivoModeloId = dispositivo.DispositivoModeloId;
                    dispositivoOriginal.DispositivoTipoId = dispositivo.DispositivoTipoId;
                    dispositivoOriginal.MAC = dispositivo.MAC;
                    dispositivoOriginal.UserId = userId;

                    // Cod_Dispositivo NÃO é alterado
                    // dispositivoOriginal.Cod_Dispositivo = dispositivoOriginal.Cod_Dispositivo;

                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Dispositivos.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["DispositivoMarcaId"] = new SelectList(_context.DispositivoMarcas, "Id", "MarcasDispositivo", dispositivo.DispositivoMarcaId);
            ViewData["DispositivoModeloId"] = new SelectList(_context.DispositivoModelos, "Id", "Modelo", dispositivo.DispositivoModeloId);
            ViewData["DispositivoTipoId"] = new SelectList(_context.DispositivoTipos, "Id", "TiposDispositivo", dispositivo.DispositivoTipoId);

            return PartialView("_Edit", dispositivo);
        }

        // POST: Dtti/Dispositivos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var dispositivo = await _context.Dispositivos.FindAsync(id);
            if (dispositivo == null)
            {
                return Json(new { success = false, message = "Dispositivo não encontrado." });
            }

            try
            {
                _context.Dispositivos.Remove(dispositivo);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Dispositivo eliminado com sucesso!" });
            }
            catch (Exception ex)
            {
                // Log do erro se necessário
                return Json(new { success = false, message = $"Erro ao eliminar o dispositivo: {ex.Message}" });
            }
        }
    }
}
