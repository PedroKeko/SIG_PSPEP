using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIG_PSPEP.Services
{
    public interface ISeedSituacaoEfectivoInitial
    {
        Task SeedSituacoesEfectivoAsync();
    }

    public class SeedSituacaoEfectivoInitial : ISeedSituacaoEfectivoInitial
    {
        private readonly AppDbContext _context;

        public SeedSituacaoEfectivoInitial(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedSituacoesEfectivoAsync()
        {
            if (!await _context.SituacaoEfectivos.AnyAsync())
            {
                var situacoes = new List<SituacaoEfectivo>
                {
                    new SituacaoEfectivo { TipoSituacao = "Activo" },
                    new SituacaoEfectivo { TipoSituacao = "Passivo" }
                };

                await _context.SituacaoEfectivos.AddRangeAsync(situacoes);
                await _context.SaveChangesAsync();
            }
        }
    }
}
