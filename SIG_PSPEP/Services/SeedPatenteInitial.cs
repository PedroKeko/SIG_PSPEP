using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Services
{
    public class SeedPatenteInitial : ISeedPatenteInitial
    {
        private readonly AppDbContext _context;

        public SeedPatenteInitial(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedPatentesAsync()
        {
            if (!await _context.Patentes.AnyAsync())
            {
                var patentes = new List<Patente>
                {
                    // Oficiais Comissários
                  new Patente { Posto = "Comissário-Geral", Classe = "Oficial Comissários" },
                  new Patente { Posto = "Comissário-Chefe", Classe = "Oficial Comissários" },
                  new Patente { Posto = "Comissário", Classe = "Oficial Comissários" },
                  new Patente { Posto = "Subcomissário", Classe = "Oficial Comissários" },
                   // Oficiais Superiores
                  new Patente { Posto = "Superintendente-Chefe", Classe = "Oficial Superior" },
                  new Patente { Posto = "Superintendente", Classe = "Oficial Superior" },
                  new Patente { Posto = "Intendente", Classe = "Oficial Superior" },

                   // Oficiais Subalternos
                  new Patente { Posto = "Inspector-Chefe", Classe = "Oficial Subalterno" },
                  new Patente { Posto = "Inspector", Classe = "Oficial Subalterno" },
                  new Patente { Posto = "Subinspector", Classe = "Oficial Subalterno" },
                   // Subchefes
                  new Patente { Posto = "1º Subchefe", Classe = "Subchefe" },
                  new Patente { Posto = "2º Subchefe", Classe = "Subchefe" },
                  new Patente { Posto = "3º Subchefe", Classe = "Subchefe" },
                  // Agentes (Executantes)
                  new Patente { Posto = "Agente de 1ª Classe", Classe = "Agente" },
                  new Patente { Posto = "Agente de 2ª Classe", Classe = "Agente" },
                  new Patente { Posto = "Agente de 3ª Classe", Classe = "Agente" }
                };

                await _context.Patentes.AddRangeAsync(patentes);
                await _context.SaveChangesAsync();
            }
        }
    }
}
