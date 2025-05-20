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
                  new Patente { Posto = "Comissário-Geral", Classe = "Oficial Comissário", grau=16, Diuturnidade = 0 },
                  new Patente { Posto = "Comissário-Chefe", Classe = "Oficial Comissário", grau=15, Diuturnidade = 0 },
                  new Patente { Posto = "Comissário", Classe = "Oficial Comissário", grau=14, Diuturnidade = 0 },
                  new Patente { Posto = "Subcomissário", Classe = "Oficial Comissário", grau=13, Diuturnidade = 0 },
                   // Oficiais Superiores
                  new Patente { Posto = "Superintendente-Chefe", Classe = "Oficial Superior", grau=12, Diuturnidade = 4 },
                  new Patente { Posto = "Superintendente", Classe = "Oficial Superior", grau=11, Diuturnidade = 4 },
                  new Patente { Posto = "Intendente", Classe = "Oficial Superior", grau=10, Diuturnidade = 4 },

                   // Oficiais Subalternos
                  new Patente { Posto = "Inspector-Chefe", Classe = "Oficial Subalterno", grau=9, Diuturnidade = 3 },
                  new Patente { Posto = "Inspector", Classe = "Oficial Subalterno", grau=8, Diuturnidade = 3 },
                  new Patente { Posto = "Subinspector", Classe = "Oficial Subalterno", grau=7, Diuturnidade = 3 },
                   // Subchefes
                  new Patente { Posto = "1º Subchefe", Classe = "Subchefe", grau=6, Diuturnidade = 3 },
                  new Patente { Posto = "2º Subchefe", Classe = "Subchefe", grau=5, Diuturnidade = 3 },
                  new Patente { Posto = "3º Subchefe", Classe = "Subchefe", grau=4, Diuturnidade = 3 },
                  // Agentes (Executantes)
                  new Patente { Posto = "Agente de 1ª Classe", Classe = "Agente", grau=3, Diuturnidade = 3 },
                  new Patente { Posto = "Agente de 2ª Classe", Classe = "Agente", grau=2, Diuturnidade = 5 },
                  new Patente { Posto = "Agente de 3ª Classe", Classe = "Agente", grau=1, Diuturnidade = 4 }
                };

                await _context.Patentes.AddRangeAsync(patentes);
                await _context.SaveChangesAsync();
            }
        }
    }
}
