using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIG_PSPEP.Services
{
    public interface ISeedEfectividadeTipoInitial
    {
        Task SeedEfectividadeTiposAsync();
    }

    public class SeedEfectividadeTipoInitial : ISeedEfectividadeTipoInitial
    {
        private readonly AppDbContext _context;

        public SeedEfectividadeTipoInitial(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedEfectividadeTiposAsync()
        {
            if (!await _context.EfectividadeTipos.AnyAsync())
            {
                var tipos = new List<EfectividadeTipo>
                {
                    new EfectividadeTipo { DescTipo = "Presente", Sigla = "P" },
                    new EfectividadeTipo { DescTipo = "Ausente", Sigla = "A" },                    
                    new EfectividadeTipo { DescTipo = "Dispençado", Sigla = "D" },
                    new EfectividadeTipo { DescTipo = "Ausencia Justificada", Sigla = "AJ" }
                };

                await _context.EfectividadeTipos.AddRangeAsync(tipos);
                await _context.SaveChangesAsync();
            }
        }
    }
}
