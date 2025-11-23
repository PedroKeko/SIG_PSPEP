using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Services
{
    public interface ISeedInitial
    {
        Task SeedOrgaoUnidadesAsync();
        Task SeedOrgUnidPnaMinintAsync();
    }

    public class SeedInitial : ISeedInitial
    {
        private readonly AppDbContext _context;

        public SeedInitial(AppDbContext context)
        {
            _context = context;
        }

        // Seed para OrgaoUnidade
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
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Telecomunicações e Tecnologia de Informação", Sigla = "DTTI" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Pessoal e Quadro", Sigla = "DPQ" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Logística", Sigla = "DL" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Transporte", Sigla = "DT" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Inspecção", Sigla = "DI" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Educação Patriotica", Sigla = "DEPAT" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Finanças", Sigla = "DAF" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Administração e Serviços", Sigla = "DAS" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Acessória Jurídica", Sigla = "DAJ" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto de Estudo e Planeamento", Sigla = "DEP" },
                    new OrgaoUnidade { NomeOrgaoUnidade = "Departameto das Operações", Sigla = "DOP" }
                };

                await _context.OrgaoUnidades.AddRangeAsync(orgaosUnidades);
                await _context.SaveChangesAsync();
            }
        }

        // Seed para OrgUnidPnaMinint
        public async Task SeedOrgUnidPnaMinintAsync()
        {
            if (!await _context.OrgUnidPnaMinints.AnyAsync())
            {
                var orgPnaMinint = new List<OrgUnidPnaMinint>
                {
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Ministério do Interior", NomeOrgaoUnidade = "Centro Integrado de Segurança Pública", Sigla = "CISP/MININT" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Ministério do Interior", NomeOrgaoUnidade = "Direcção de Telecomunicações e Tecnologia de Informação", Sigla = "DTTI/MININT" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Comando Geral", NomeOrgaoUnidade = "Polícia de Intervenção Rápida", Sigla = "PIR/CGPN" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Comando Geral", NomeOrgaoUnidade = "Polícia de Guarda Fronteira", Sigla = "PGF/CGPN" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Comando Geral", NomeOrgaoUnidade = "Polícia de Fiscal Aduaneira", Sigla = "PFA/CGPN" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Comando Geral", NomeOrgaoUnidade = "Polícia de Segurança Pessoal e de Entidades Protocolares", Sigla = "PSPEP/CGPN" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Comando Geral", NomeOrgaoUnidade = "Polícia de Segurança e Objectivos Estratégicos", Sigla = "PSOE/CGPN" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Comando Geral", NomeOrgaoUnidade = "Direcção de Telecomunicações e Tecnologia de Informação", Sigla = "DTTI/CGPN" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Comando Geral", NomeOrgaoUnidade = "Direcção de Transito e Segurança Rodoviária", Sigla = "DTSER/CGPN" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Comando Geral", NomeOrgaoUnidade = "Direcção de Investigação e Ilícitos Penais", Sigla = "DIIP/CGPN" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Comando Geral", NomeOrgaoUnidade = "Direcção de Logística", Sigla = "DL/CGPN" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Comando Geral", NomeOrgaoUnidade = "Direcção de Pessoal e Quadro", Sigla = "DPQ/CGPN" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Gabinete do Comandante", Sigla = "GABCMDTE/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Gabinete do 2º Comandante", Sigla = "GAB2ºCMDTE/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Gabinete do Chefe Estado Maior", Sigla = "GABCEM/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Unidade de Proteção Parlamentar", Sigla = "UPP/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Unidade de Proteção Judicial", Sigla = "UPJ/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Unidade de Proteção e Entidades Protocolar", Sigla = "UPEP/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Unidade de Proteção Diplomática", Sigla = "UPD/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Unidade de Guarda de Honrra e Honorifica", Sigla = "UGHH/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Unidade de Banda de Música", Sigla = "UBM/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Departameto de Telecomunicações e Tecnologia de Informação", Sigla = "DTTI/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Departameto de Pessoal e Quadro", Sigla = "DPQ/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Departameto de Logística", Sigla = "DL/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Departameto de Transporte", Sigla = "DT/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Departameto de Inspecção", Sigla = "DI/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Departameto de Educação Patriotica", Sigla = "DEPAT/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Departameto de Finanças", Sigla = "DAF/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Departameto de Administração e Serviços", Sigla = "DAS/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Departameto de Acessória Jurídica", Sigla = "DAJ/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Departameto de Estudo e Planeamento", Sigla = "DEP/PSPEP" },
                    new OrgUnidPnaMinint { OrgaoUnidadePrincipal = "Polícia de Segurança Pessoal e de Entidades Protocolares", NomeOrgaoUnidade = "Departameto das Operações", Sigla = "DOP/PSPEP" }
                };

                await _context.OrgUnidPnaMinints.AddRangeAsync(orgPnaMinint);
                await _context.SaveChangesAsync();
            }
        }
    }
}
