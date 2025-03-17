using FastReport.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connection));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
          .AddEntityFrameworkStores<AppDbContext>();
// Registrar o serviço CompressaoImagem
builder.Services.AddScoped<ImageCompressionService>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 3;
    options.Password.RequireNonAlphanumeric = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Conta/Login"; // Define o caminho para login
    options.LogoutPath = "/Conta/Sair"; // Opcional: define o caminho para logout
    options.AccessDeniedPath = "/Conta/AcessoNegado"; // Opcional: caminho para acesso negado
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "AspNetCore.Cookies"; //Tempo de Espiração do cockie
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Require_Admin_ChDepar",
         policy => policy.RequireRole("Administrador", "Chefe de Departamento"));

    options.AddPolicy("Require_Admin_ChDepar_ChSec",
        policy => policy.RequireRole("Administrador", "Chefe de Departamento", "Chefe de Secção"));

    options.AddPolicy("Require_Admin_ChDepar_ChSec_Esp",
        policy => policy.RequireRole("Administrador", "Chefe de Departamento", "Chefe de Secção", "Especialista"));

    options.AddPolicy("Require_Admin_ChDepar_ChSec_Esp",
        policy => policy.RequireRole("Administrador", "Chefe de Departamento", "Chefe de Secção", "Especialista", "Usuario Comum"));
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdminClaimAccess",
        policy => policy.RequireClaim("CadastradoEm"));

    options.AddPolicy("IsAdminClaimAccess",
        policy => policy.RequireClaim("IsAdmin", "true"));

    options.AddPolicy("IsFuncionarioClaimAccess",
       policy => policy.RequireClaim("IsFuncionario", "true"));


    options.AddPolicy("TesteClaim",
    policy => policy.RequireClaim("Teste", "teste_claim"));
});

//builder.Services.AddScoped<IAuthorizationHandler, TempoCadastroHandler>();
builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();
builder.Services.AddScoped<ISeedUserClaimsInitial, SeedUserClaimsInitial>();

////Registando Sql Connection para o Fast Report
FastReport.Utils.RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));
builder.Services.AddFastReport(); // Adicione esta linha


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseFastReport(); // Middleware do FastReport

await CriarPerfisUsuariosAsync(app);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "MinhaArea",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task CriarPerfisUsuariosAsync(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory?.CreateScope())
    {
        var service = scope?.ServiceProvider.GetService<ISeedUserRoleInitial>();
        await service.SeedRolesAsync();
        await service.SeedUsersAsync();
    }
}

var builder1 = WebApplication.CreateBuilder(args);

// Adiciona os serviços de CORS
builder1.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

