using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Areas.Admin.Models;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrador")]
public class AdminUsersController : Controller
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly UserManager<IdentityUser> userManager;
    private readonly AppDbContext _context;

    public AdminUsersController(AppDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
    {
        this.userManager = userManager;
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var usuarios = await _context.UsuarioAutes
            .Where(u => u.EfectivoId != null)
            .Include(u => u.User)
            .Include(u => u.Efectivo)
            .ThenInclude(e => e.FuncaoCargo)
            .Include(u => u.Efectivo.Patente)
            .Include(u => u.Efectivo.OrgaoUnidade)
            .Include(u => u.Area)
            .OrderByDescending(u => u.Id)
            .ToListAsync();

        var model = new List<UsuarioViewModel>();

        foreach (var usuarioAute in usuarios)
        {
            var user = await userManager.FindByIdAsync(usuarioAute.UserId);
            var roles = await userManager.GetRolesAsync(user);

            var foto = await _context.FotoEfectivos
                .Where(f => f.EfectivoId == usuarioAute.EfectivoId)
                .Select(f => f.Foto)
                .FirstOrDefaultAsync();

            var fotoBase64 = foto != null ? Convert.ToBase64String(foto) : null; // Garantir que a foto esteja em base64

            model.Add(new UsuarioViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.PhoneNumber,
                NomeEfectivo = usuarioAute.Efectivo?.NomeCompleto,
                Area = usuarioAute.Area?.NomeArea,
                Estado = user.LockoutEnabled,
                FotoBase64 = fotoBase64,
                Roles = roles.ToList()
            });
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user == null)
        {
            ViewBag.ErrorMessage = $"Usuário com Id = {id} não foi encontrado";
            return View("NotFound");
        }
        else
        {
            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("Index");
        }
    }

    [Authorize(Policy = "Require_Admin_ChDepar")]
    public async Task<IActionResult> Registro(Models.RegistroViewModel model)
    {
        var efectivosSemUsuario = _context.Efectivos
           .Where(e => !_context.UsuarioAutes.Any(ua => ua.EfectivoId == e.Id))
           .Select(e => new
           {
               Id = e.Id,
               NomeCompleto = e.NomeCompleto
           });

        var todasAareas = _context.Areas.Select(e => new
        {
            Id = e.Id,
            NomeArea = e.NomeArea
        });

        ViewData["EfectivoId"] = new SelectList(efectivosSemUsuario, "Id", "NomeCompleto");
        ViewData["AreaId"] = new SelectList(todasAareas, "Id", "NomeArea");
        if (ModelState.IsValid)
        {
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            // Cria o usuário na tabela AspNetUsers
            var result = await userManager.CreateAsync(user, model.Password);

            // Se o usuário foi criado com sucesso
            if (result.Succeeded)
            {
                // Cria o registro na tabela UsuarioAute
                var usuarioAute = new UsuarioAute
                {
                    EfectivoId = model.EfectivoId,
                    AreaId = model.AreaId,
                    UserId = user.Id
                };

                // Adiciona o novo registro ao contexto e salva
                _context.UsuarioAutes.Add(usuarioAute);
                await _context.SaveChangesAsync();

                // Redireciona para a página inicial ou onde desejar
                return RedirectToAction("Index", "AdminUsers", "Admin");
            }

            // Se houver erros, adiciona ao ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }


    [HttpGet]
    [Authorize(Policy = "Require_Admin_ChDepar")]
    public async Task<IActionResult> EditUser(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var usuarioAute = await _context.UsuarioAutes
            .Include(u => u.Efectivo)
            .Include(u => u.Area)
            .FirstOrDefaultAsync(u => u.UserId == id);

        if (usuarioAute == null)
            return NotFound();

        var user = await userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var model = new RegistroViewModel
        {
            UserId = user.Id,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            EstadoConta = user.LockoutEnabled,
            EfectivoId = usuarioAute.EfectivoId,
            AreaId = usuarioAute.AreaId,
        };

        ViewData["EfectivoId"] = new SelectList(_context.Efectivos, "Id", "NomeCompleto", model.EfectivoId);
        ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "NomeArea", model.AreaId);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Require_Admin_ChDepar")]
    public async Task<IActionResult> Edit(RegistroViewModel model)
    {
        if (string.IsNullOrEmpty(model.UserId))
            return NotFound();

        if (!ModelState.IsValid)
        {
            CarregarSelects(model);
            return View("EditUser", model);
        }

        var user = await userManager.FindByIdAsync(model.UserId);
        if (user == null)
            return NotFound();

        // Atualiza os dados principais do usuário
        user.Email = model.Email;
        user.UserName = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        user.LockoutEnabled = model.EstadoConta;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            CarregarSelects(model);
            return View("EditUser", model);
        }

        // Trocar senha manualmente (se nova senha foi fornecida)
        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            try
            {
                var hasPassword = await userManager.HasPasswordAsync(user);
                if (hasPassword)
                {
                    var removeResult = await userManager.RemovePasswordAsync(user);
                    if (!removeResult.Succeeded)
                    {
                        foreach (var error in removeResult.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);

                        CarregarSelects(model);
                        return View("EditUser", model);
                    }
                }

                var addPasswordResult = await userManager.AddPasswordAsync(user, model.Password);
                if (!addPasswordResult.Succeeded)
                {
                    foreach (var error in addPasswordResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    CarregarSelects(model);
                    return View("EditUser", model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Erro ao redefinir a senha: " + ex.Message);
                CarregarSelects(model);
                return View("EditUser", model);
            }
        }

        // Atualiza os dados na tabela UsuarioAutes
        var usuarioAute = await _context.UsuarioAutes.FirstOrDefaultAsync(u => u.UserId == model.UserId);
        if (usuarioAute != null)
        {
            usuarioAute.AreaId = model.AreaId;

            _context.UsuarioAutes.Update(usuarioAute);
            await _context.SaveChangesAsync();
        }

        // Redireciona para a página de listagem
        return RedirectToAction("Index", "AdminUsers", new { area = "Admin" });
    }

    private void CarregarSelects(RegistroViewModel model)
    {
        ViewData["EfectivoId"] = new SelectList(_context.Efectivos, "Id", "NomeCompleto", model.EfectivoId);
        ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "NomeArea", model.AreaId);
    }
}
