using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidade;

namespace SIG_PSPEP.Services
{
    public interface ISeedProvinciaInitial
    {
        Task SeedProvinciaAsync();
    }
    public class SeedProvinciaInitial : ISeedProvinciaInitial
    {
        private readonly AppDbContext _context;

        public SeedProvinciaInitial(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedProvinciaAsync()
        {
            if (!await _context.Provincias.AnyAsync())
            {
                var provincias = new List<Provincia>
                {
                    new Provincia { Nome = "Bengo" },
                    new Provincia { Nome = "Benguela" },
                    new Provincia { Nome = "Cabinda" },
                    new Provincia { Nome = "Cuanza Norte" },
                    new Provincia { Nome = "Cuanza Sul" },
                    new Provincia { Nome = "Cunene" },
                    new Provincia { Nome = "Cuando" },
                    new Provincia { Nome = "Cubango" },
                    new Provincia { Nome = "Huila" },
                    new Provincia { Nome = "Icolo-Ibengo" },
                    new Provincia { Nome = "Luanda" },
                    new Provincia { Nome = "Lunda Norte" },
                    new Provincia { Nome = "Lunda Sul" },
                    new Provincia { Nome = "Malanje" },
                    new Provincia { Nome = "Moxico" },
                    new Provincia { Nome = "Moxico-Leste" },
                    new Provincia { Nome = "Namibe" },
                    new Provincia { Nome = "Uíge" },
                    new Provincia { Nome = "Zaire" }
                };

                await _context.Provincias.AddRangeAsync(provincias);
                await _context.SaveChangesAsync();
            }
        }
    }
}
