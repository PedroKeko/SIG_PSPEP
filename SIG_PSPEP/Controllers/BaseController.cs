using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using SIG_PSPEP.Entidades;

public class BaseController : Controller
{
    protected readonly AppDbContext _context;
    protected string? userId;
    protected UsuarioAute? usuarioAute;

    public BaseController(AppDbContext context)
    {
        _context = context;
    }

    // Este método será executado antes de cada ação
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        usuarioAute = _context.UsuarioAutes
            .Include(u => u.Area)
            .FirstOrDefault(u => u.UserId == userId);

        base.OnActionExecuting(context);
    }

    // Método para checar o acesso de área
    protected bool UsuarioTemAcessoArea(string areaPermitida)
    {
        return usuarioAute?.Area?.NomeArea == areaPermitida;
    }
}
