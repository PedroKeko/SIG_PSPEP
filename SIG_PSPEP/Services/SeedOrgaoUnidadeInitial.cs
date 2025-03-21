using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIG_PSPEP.Services
{
    public interface ISeedOrgaoUnidadeInitial
    {
        Task SeedOrgaoUnidadesAsync();
    }

    public class SeedOrgaoUnidadeInitial : ISeedOrgaoUnidadeInitial
    {
        private readonly AppDbContext _context;

        public SeedOrgaoUnidadeInitial(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedOrgaoUnidadesAsync()
        {
            if (!await _context.OrgaoUnidades.AnyAsync())
            {
                var orgaosUnidades = new List<OrgaoUnidade>
                {
                    new OrgaoUnidade { NomeOrgaoUnidade = "Gabinete do Comandante", Sigla = "GABCMDTE" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Gabinete do 2º Comandante", Sigla = "GAB2ºCMDTE" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Gabinete do Chefe Estado Maior", Sigla = "GABCEM" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Unidade de Proteção Parlamentar", Sigla = "UPP" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Unidade de Proteção Judicial", Sigla = "UPJ" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Unidade de Proteção e Entidades Protocolar", Sigla = "UPEP" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Unidade de Proteção Diplomática", Sigla = "UPD" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Unidade de Guarda de Honrra e Honorifica", Sigla = "UGHH" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Unidade de Banda de Música", Sigla = "UBM" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Logística", Sigla = "DL" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Transporte", Sigla = "DT" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Inspecção", Sigla = "DI" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Educação Patriotica", Sigla = "DEPAT" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Finanças", Sigla = "DF" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Administração e Serviços", Sigla = "DAS" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Acessória Jurídica", Sigla = "DAJ" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Estudo e Planeamento", Sigla = "DEP" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto das Operações", Sigla = "DOP" }
                };

                await _context.OrgaoUnidades.AddRangeAsync(orgaosUnidades);
                await _context.SaveChangesAsync();
            }
        }
    }
}
