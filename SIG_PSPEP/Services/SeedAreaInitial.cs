using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Services
{
    public class SeedAreaInitial : ISeedAreaInitial
    {
        private readonly AppDbContext _context;

        public SeedAreaInitial(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedAreasAsync()
        {
            if (!await _context.Areas.AnyAsync())
            {
                var areas = new List<Area>
                {
                    new Area { NomeArea = "DPQ", DescricaoArea = "Departamento de Pessoal e Quadro" },
                    new Area { NomeArea = "DSS", DescricaoArea = "Departamento de Serviços de Saúde" },
                    new Area { NomeArea = "DT", DescricaoArea = "Departamento de Transporte" },
                    new Area { NomeArea = "DL", DescricaoArea = "Departamento de Logística" },
                    new Area { NomeArea = "DTTI", DescricaoArea = "Departamento de Telecomunicação e de Tecnologia de Informação" }
                };

                await _context.Areas.AddRangeAsync(areas);
                await _context.SaveChangesAsync();
            }
        }
    }
}
