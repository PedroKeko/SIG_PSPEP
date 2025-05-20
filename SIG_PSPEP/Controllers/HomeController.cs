using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Areas.Admin.Models;
using SIG_PSPEP.Context;
using SIG_PSPEP.Models;

namespace SIG_PSPEP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Busca o usuário logado e carrega todos os relacionamentos necessários
            var usuarioAute = await _context.UsuarioAutes
                .Include(u => u.Efectivo)
                    .ThenInclude(e => e.OrgaoUnidade)
                .Include(u => u.Efectivo)
                    .ThenInclude(e => e.FuncaoCargo)
                .Include(u => u.Efectivo)
                    .ThenInclude(e => e.Patente)
                .Include(u => u.Efectivo)
                    .ThenInclude(e => e.ProvinciaNascimento)
                .Include(u => u.Efectivo)
                    .ThenInclude(e => e.ProvinciaResidencia)
                .Include(u => u.Efectivo)
                    .ThenInclude(e => e.Municipio)
                .Include(u => u.Efectivo)
                    .ThenInclude(e => e.SituacaoEfectivo)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (usuarioAute?.Efectivo == null)
            {
                return NotFound(); // ou RedirectToAction("Index", "Home");
            }

            var efectivo = usuarioAute.Efectivo;

            var historicos = await _context.EfectivoHistoricos
                .Where(h => h.EfectivoId == efectivo.Id)
                .OrderByDescending(h => h.Data)
                .ToListAsync();

            var foto = await _context.FotoEfectivos
                .Where(f => f.EfectivoId == efectivo.Id)
                .Select(f => f.Foto)
                .FirstOrDefaultAsync();

            int idade = DateTime.Today.Year - efectivo.DataNasc.Year;
            if (efectivo.DataNasc.Date > DateTime.Today.AddYears(-idade))
            {
                idade--;
            }
            int tempoServico = DateTime.Today.Year - efectivo.DataIngresso.Year;
            if (efectivo.DataIngresso.Date > DateTime.Today.AddYears(-tempoServico))
            {
                tempoServico--;
            }

            var model = new PortalEfetivoViewModel
            {
                Id = efectivo.Id,
                SituacaoEfectivo = efectivo.SituacaoEfectivo?.TipoSituacao ?? "Não definido",
                OrgaoUnidade = efectivo.OrgaoUnidade?.NomeOrgaoUnidade ?? "Não definido",
                SiglaUnidade = efectivo.OrgaoUnidade?.Sigla ?? "Não definido",
                FuncaoCargo = efectivo.FuncaoCargo?.NomeFuncaoCargo ?? "Não definido",
                Patente = efectivo.Patente?.Posto ?? "Não definido",
                ProvinciaNasc = efectivo.ProvinciaNascimento?.Nome ?? "Não definido",
                ProvinciaRes = efectivo.ProvinciaResidencia?.Nome ?? "Não definido",
                Municipio = efectivo.Municipio?.Nome ?? "Não definido",
                Num_Processo = efectivo.Num_Processo,
                NIP = efectivo.NIP,
                N_Agente = efectivo.N_Agente,
                NomeCompleto = efectivo.NomeCompleto,
                Apelido = efectivo.Apelido,
                Genero = efectivo.Genero,
                DataNasc = efectivo.DataNasc,
                Idade = idade,
                EstadoCivil = efectivo.EstadoCivil,
                GSanguineo = efectivo.GSanguineo,
                NumBI = efectivo.NumBI,
                BIValidade = efectivo.BIValidade,
                BIEmitido = efectivo.BIEmitido,
                NumCartaConducao = efectivo.NumCartaConducao,
                CartaValidade = efectivo.CartaValidade,
                CartaEmitido = efectivo.CartaEmitido,
                NumPassaporte = efectivo.NumPassaporte,
                PassapValidade = efectivo.PassapValidade,
                PassapEmitido = efectivo.PassapEmitido,
                Nacionalidade = efectivo.Nacionalidade,
                Destrito_BairroRes = efectivo.Destrito_BairroRes,
                Rua = efectivo.Rua,
                CasaNum = efectivo.CasaNum,
                Habilitacao = efectivo.Habilitacao,
                CursoHabilitado = efectivo.CursoHabilitado,
                InstitAcademica = efectivo.InstitAcademica,
                Telefone1 = efectivo.Telefone1,
                Telefone2 = efectivo.Telefone2,
                Email = efectivo.Email,
                DataIngresso = efectivo.DataIngresso,
                TempoServico = tempoServico,
                TipoVinculo = efectivo.TipoVinculo,
                Carreira = efectivo.Carreira,
                UnidadeOrigem = efectivo.UnidadeOrigem,
                OutrasInfo = efectivo.OutrasInfo,
                FotoByte = foto,
                Historicos = historicos
            };

            return View("Index", model);
        }

        // Criar solicitação de documento
        [HttpPost]
        public async Task<IActionResult> CriarSolicitacao(string tipoDocumento)
        {
            // Lógica para criar uma solicitação de documento no banco de dados
            // A parte de solicitação deve ser implementada conforme suas regras de negócio.

            return RedirectToAction(nameof(Index));
        }

        // Enviar mensagem para outro efetivo
        [HttpPost]
        public async Task<IActionResult> EnviarMensagem(string destinatario, string mensagem)
        {
            // Lógica para enviar mensagem ao destinatário.
            // Você deve adicionar a lógica para criar e persistir a mensagem.

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Solicitacao()
        {
            return View();
        }
        public IActionResult Pedido()
        {
            return View();
        }
    }
}
