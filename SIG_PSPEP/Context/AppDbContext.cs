using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Context;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {

    }

    public DbSet<AgregadoFam> AgregadoFams { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<Banco> Bancos { get; set; }
    public DbSet<Efectivo> Efectivos { get; set; }
    public DbSet<EfectivoContaBancaria> EfectivoContaBancarias { get; set; }
    public DbSet<EfectivoEstadoEfectividade> EfectivoEstadoEfectividades { get; set; }
    public DbSet<EfectivoOrdemServico> EfectivoOrdemServicos { get; set; }
    public DbSet<EstadoEfectividade> EstadoEfectividades { get; set; }
    public DbSet<Filiacao> Filiacaos { get; set; }
    public DbSet<FotoEfectivo> FotoEfectivos { get; set; }
    public DbSet<FuncaoCargo> FuncaoCargos { get; set; }
    public DbSet<OrdemServico> OrdemServicos { get; set; }
    public DbSet<OrgaoUnidade> OrgaoUnidades { get; set; }
    public DbSet<Patente> Patentes { get; set; }
    public DbSet<SituacaoEfectivo> SituacaoEfectivos { get; set; }
    public DbSet<UsuarioAute> UsuarioAutes { get; set; }

    #region
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // 🔹 Necessário para Identity funcionar

        modelBuilder.Entity<EfectivoOrdemServico>()
            .HasOne(e => e.Patente)
            .WithMany()
            .HasForeignKey(e => e.PatenteId)
            .OnDelete(DeleteBehavior.NoAction);
    }


    #endregion

}
