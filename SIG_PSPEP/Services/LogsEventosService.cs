using Microsoft.AspNetCore.Identity;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Services
{
    public class LogsEventosService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LogsEventosService> _logger;

        public LogsEventosService(AppDbContext context, UserManager<IdentityUser> userManager, ILogger<LogsEventosService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task LogEventAsync(string userId, string tipoEvento, string obs)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tipoEvento) || string.IsNullOrEmpty(obs))
            {
                _logger.LogWarning("Tentativa de registrar evento com dados inválidos");
                return; // Não faz o log se algum dado importante estiver faltando.
            }

            try
            {
                var logEvento = new LogsEvento
                {
                    UserId = userId,
                    TipoEvento = tipoEvento,
                    Obs = obs,
                    DataRegisto = DateTime.Now
                };

                _context.LogsEventos.Add(logEvento);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao registrar evento: {ex.Message}", ex);
            }
        }

        internal async Task LogEventAsync(object id, string v1, string v2)
        {
            throw new NotImplementedException();
        }

        internal async Task LogEventAsync(string v1, string v2)
        {
            throw new NotImplementedException();
        }
    }
}
