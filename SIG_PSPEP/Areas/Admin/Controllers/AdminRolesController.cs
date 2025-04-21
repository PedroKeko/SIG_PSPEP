using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using SIG_PSPEP.Areas.Admin.Models;
using SIG_PSPEP.Models;
using SIG_PSPEP.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;


namespace SIG_PSPEP.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrador")]
public class AdminRolesController : Controller
{
    private RoleManager<IdentityRole> roleManager;
    private UserManager<IdentityUser> userManager;
    private readonly AppDbContext _context;

    public AdminRolesController(RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager, AppDbContext context)
    {
        this.roleManager = roleManager;
        this.userManager = userManager;
        _context = context;
    }



    public async Task<IActionResult> Index()
    {
        var roles = await _context.Roles
            .Select(r => new RoleModification
            {
                RoleId = r.Id,
                RoleName = r.Name,
                UserCount = _context.UserRoles.Count(ur => ur.RoleId == r.Id)
            })
            .OrderBy(r => r.RoleName)
            .ToListAsync();

        return View(roles);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RoleModification model)
    {
        if (!ModelState.IsValid)
        {
            var erros = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, errors = erros });
        }

        if (await roleManager.RoleExistsAsync(model.RoleName))
        {
            return Json(new { success = false, errors = new[] { "Esta função já existe." } });
        }

        var result = await roleManager.CreateAsync(new IdentityRole(model.RoleName));

        if (result.Succeeded)
        {
            return Json(new { success = true, message = "Função criada com sucesso!" });
        }

        return Json(new { success = false, errors = result.Errors.Select(e => e.Description) });
    }

    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        IdentityRole role = await roleManager.FindByIdAsync(id);

        List<IdentityUser> members = new List<IdentityUser>();
        List<IdentityUser> nonmembers = new List<IdentityUser>();
        foreach (IdentityUser user in userManager.Users)
        {
            var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonmembers;
            list.Add(user);
        }

        return PartialView(new RoleEdit
        {
            Role = role,
            Members = members,
            NonMembers = nonmembers
        });
    }

    [HttpPost]
    public async Task<IActionResult> Update(RoleModification model)
    {
        IdentityResult result;

        if (ModelState.IsValid)
        {
            foreach (string userId in model.AddIds ?? new string[] { })
            {
                IdentityUser user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    result = await userManager.AddToRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                        Errors(result);
                }
            }
            foreach (string userId in model.DeleteIds ?? new string[] { })
            {
                IdentityUser user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    result = await userManager.RemoveFromRoleAsync(user, model.RoleName);
                    if (!result.Succeeded)
                        Errors(result);
                }
            }
        }
        if (ModelState.IsValid)
            return RedirectToAction(nameof(Index));
        else
            return await Update(model.RoleId);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role == null)
            return Json(new { success = false, message = "Função não encontrada." });

        var result = await roleManager.DeleteAsync(role);
        if (result.Succeeded)
            return Json(new { success = true, message = "Função excluída com sucesso!" });

        return Json(new { success = false, message = "Erro ao excluir a função." });
    }

    private void Errors(IdentityResult result)
    {
        foreach (IdentityError error in result.Errors)
            ModelState.AddModelError("", error.Description);
    }

}
