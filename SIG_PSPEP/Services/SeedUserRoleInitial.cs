using Microsoft.AspNetCore.Identity;

namespace SIG_PSPEP.Services;

public class SeedUserRoleInitial : ISeedUserRoleInitial
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;


    public SeedUserRoleInitial(UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedRolesAsync()
    {
        if (!await _roleManager.RoleExistsAsync("Administrador"))
        {
            IdentityRole role = new IdentityRole();
            role.Name = "Administrador";
            role.NormalizedName = "ADMINISTRADOR";
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            IdentityResult roleResult = await _roleManager.CreateAsync(role);
        }

        if (!await _roleManager.RoleExistsAsync("Chefe de Departamento"))
        {
            IdentityRole role = new IdentityRole();
            role.Name = "Chefe de Departamento";
            role.NormalizedName = "CHEFE DE DEPARTAMENTO";
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            IdentityResult roleResult = await _roleManager.CreateAsync(role);
        }

        if (!await _roleManager.RoleExistsAsync("Chefe de Secção"))
        {
            IdentityRole role = new IdentityRole();
            role.Name = "Chefe de Secção";
            role.NormalizedName = "CHEFE DE SECÇÃO";
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            IdentityResult roleResult = await _roleManager.CreateAsync(role);
        }

        if (!await _roleManager.RoleExistsAsync("Especialista"))
        {
            IdentityRole role = new IdentityRole();
            role.Name = "Especialista";
            role.NormalizedName = "ESPECIALISTA";
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            IdentityResult roleResult = await _roleManager.CreateAsync(role);
        }

        if (!await _roleManager.RoleExistsAsync("Usuario Comum"))
        {
            IdentityRole role = new IdentityRole();
            role.Name = "Usuario Comum";
            role.NormalizedName = "USUARIO COMUM";
            role.ConcurrencyStamp = Guid.NewGuid().ToString();

            IdentityResult roleResult = await _roleManager.CreateAsync(role);
        }

        if (!await _roleManager.RoleExistsAsync("Convidado"))
        {
            IdentityRole role = new IdentityRole();
            role.Name = "Convidado";
            role.NormalizedName = "CONVIDADO";
            role.ConcurrencyStamp = Guid.NewGuid().ToString();

            IdentityResult roleResult = await _roleManager.CreateAsync(role);
        }

    }

    public async Task SeedUsersAsync()
    {
        if (await _userManager.FindByEmailAsync("admin@pna.co") == null)
        {
            IdentityUser user = new IdentityUser();
            user.UserName = "admin@pspep.pn.gov.ao";
            user.Email = "admin@pspep.pn.gov.ao";
            user.NormalizedUserName = "ADMIN@PSPEP.PN.GOV.AO";
            user.NormalizedEmail = "ADMIN@PSPEP.PN.GOV.AO";
            user.EmailConfirmed = true;
            user.LockoutEnabled = false;
            user.SecurityStamp = Guid.NewGuid().ToString();

            IdentityResult result = await _userManager.CreateAsync(user, "Admin#2025");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Administrador");
            }
        }
    }
}