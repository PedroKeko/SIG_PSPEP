using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIG_PSPEP.Services
{
    public interface ISeedFuncaoCargoInitial
    {
        Task SeedFuncoesCargosAsync();
    }

    public class SeedFuncaoCargoInitial : ISeedFuncaoCargoInitial
    {
        private readonly AppDbContext _context;

        public SeedFuncaoCargoInitial(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedFuncoesCargosAsync()
        {
            if (!await _context.FuncaoCargos.AnyAsync())
            {
                var funcoesCargos = new List<FuncaoCargo>
                {
                    new FuncaoCargo { NomeFuncaoCargo = "Comandante" },
                    new FuncaoCargo { NomeFuncaoCargo = "2º  Comandante" },
                    new FuncaoCargo { NomeFuncaoCargo = "Chefe do Gabinete do Comandante" },
                    new FuncaoCargo { NomeFuncaoCargo = "Chefe do Gabinete do 2º Comandante" },
                    new FuncaoCargo { NomeFuncaoCargo = "Chefe de Departamento" },
                    new FuncaoCargo { NomeFuncaoCargo = "Chefe de Secção" },
                    new FuncaoCargo { NomeFuncaoCargo = "Comandante de Companhia" },
                    new FuncaoCargo { NomeFuncaoCargo = "Chefe de Escolta" },
                    new FuncaoCargo { NomeFuncaoCargo = "Chefe do Posto Comando" },
                    new FuncaoCargo { NomeFuncaoCargo = "Chefe do Grupo" },
                    new FuncaoCargo { NomeFuncaoCargo = "Chefe de Secretária" },
                    new FuncaoCargo { NomeFuncaoCargo = "Oficial Especialista" },
                    new FuncaoCargo { NomeFuncaoCargo = "Especialista" },
                    new FuncaoCargo { NomeFuncaoCargo = "Outros" }
                };

                await _context.FuncaoCargos.AddRangeAsync(funcoesCargos);
                await _context.SaveChangesAsync();
            }
        }
    }
}
