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
                    new Area { NomeArea = "ADMIN", DescricaoArea = "Administrador" },
                    new Area { NomeArea = "GABCMDT", DescricaoArea = "Gabinete do Comandante" },
                    new Area { NomeArea = "GAB2CMDT", DescricaoArea = "Gabinete do 2º Comandante" },
                    new Area { NomeArea = "GABCEM", DescricaoArea = "Gabinete do Chefe Estado Maior" },
                    new Area { NomeArea = "DPQ", DescricaoArea = "Departamento de Pessoal e Quadro" },
                    new Area { NomeArea = "DOP", DescricaoArea = "Departamento das Operações" },
                    new Area { NomeArea = "DEP", DescricaoArea = "Departamento de Estudo e Planeamento" },
                    new Area { NomeArea = "DF", DescricaoArea = "Departamento das Finanças" },
                    new Area { NomeArea = "DI", DescricaoArea = "Departamento de Inspensão" },
                    new Area { NomeArea = "DEPAT", DescricaoArea = "Departamento de Educação Patriotica" },
                    new Area { NomeArea = "DSS", DescricaoArea = "Departamento de Serviços de Saúde" },
                    new Area { NomeArea = "DT", DescricaoArea = "Departamento de Transporte" },
                    new Area { NomeArea = "DL", DescricaoArea = "Departamento de Logística" },
                    new Area { NomeArea = "DTTI", DescricaoArea = "Departamento de Telecomunicação e de Tecnologia de Informação" },
                    new Area { NomeArea = "UPP", DescricaoArea = "Unidade de Protecção Parlamentar" },
                    new Area { NomeArea = "UPJ", DescricaoArea = "Unidade de Protecção Judicial" },
                    new Area { NomeArea = "UPEP", DescricaoArea = "Unidade de Protecção e Entidades Protocolares" },
                    new Area { NomeArea = "UPD", DescricaoArea = "Unidade de Protecção Diplomática" },
                    new Area { NomeArea = "UGHH", DescricaoArea = "Unidade de Guarda de Honra e Honorifica" },
                    new Area { NomeArea = "UBM", DescricaoArea = "Unidade de de Banda de Musica" },
                    new Area { NomeArea = "Portal", DescricaoArea = "Portal do Efectivo" }
                };

                await _context.Areas.AddRangeAsync(areas);
                await _context.SaveChangesAsync();
            }
        }
    }
}
