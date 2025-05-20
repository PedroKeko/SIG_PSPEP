using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Entidade;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Context;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {

    }

    public DbSet<AgregadoFam> AgregadoFams { get; set; }
    public DbSet<Alimento> Alimentos { get; set; }
    public DbSet<AlimentoCategoria> AlimentoCategorias { get; set; }
    public DbSet<AlimentoControle> AlimentoControles { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<Armamento> Armamentos { get; set; }
    public DbSet<ArmamentoCondicao> ArmamentoCondicaos { get; set; }
    public DbSet<ArmamentoCondicaoTipo> ArmamentoCondicaoTipos { get; set; }
    public DbSet<ArmamentoControlUtilidade> ArmamentoControlUtilidades { get; set; }
    public DbSet<ArmamentoLocalizacao> ArmamentoLocalizacaos { get; set; }
    public DbSet<Banco> Bancos { get; set; }
    public DbSet<Dispositivo> Dispositivos { get; set; }
    public DbSet<DispositivoLocalizacao> DispositivoLocalizacoes { get; set; }
    public DbSet<DispositivoMarca> DispositivoMarcas { get; set; }
    public DbSet<DispositivoModelo> DispositivoModelos { get; set; }
    public DbSet<DispositivoTipo> DispositivoTipos { get; set; }
    public DbSet<EfectividadeTipo> EfectividadeTipos { get; set; }
    public DbSet<Efectivo> Efectivos { get; set; }
    public DbSet<EfectivoContaBancaria> EfectivoContaBancarias { get; set; }
    public DbSet<EfectivoEfectividade> EfectivoEfectividades { get; set; }
    public DbSet<EfectivoEstadoEfectividade> EfectivoEstadoEfectividades { get; set; }
    public DbSet<EfectivoHistorico> EfectivoHistoricos { get; set; }
    public DbSet<EfectivoOrdemServico> EfectivoOrdemServicos { get; set; }
    public DbSet<EstadoEfectividade> EstadoEfectividades { get; set; }
    public DbSet<Filiacao> Filiacaos { get; set; }
    public DbSet<FotoEfectivo> FotoEfectivos { get; set; }
    public DbSet<FuncaoCargo> FuncaoCargos { get; set; }
    public DbSet<LogsAcesso> LogsAcessos { get; set; }
    public DbSet<LogsEvento> LogsEventos { get; set; }
    public DbSet<MensagesAdmin> MensagesAdmins { get; set; }
    public DbSet<Municipio> Municipios { get; set; }
    public DbSet<OrdemServico> OrdemServicos { get; set; }
    public DbSet<OrgaoUnidade> OrgaoUnidades { get; set; }
    public DbSet<Patente> Patentes { get; set; }
    public DbSet<Provincia> Provincias { get; set; }
    public DbSet<RadioTipo> RadioTipos { get; set; }
    public DbSet<Radio> Radios{ get; set; }
    public DbSet<RadioOrgaoUnidade> RadioOrgaoUnidades { get; set; }
    public DbSet<SituacaoEfectivo> SituacaoEfectivos { get; set; }
    public DbSet<UsuarioAute> UsuarioAutes { get; set; }
    public DbSet<Utencilio> Utencilios { get; set; }
    public DbSet<UtencilioTipo> UtencilioTipos { get; set; }
    public DbSet<Vestuario> Vestuarios { get; set; }
    public DbSet<VestuarioEntrada> VestuarioEntradas { get; set; }
    public DbSet<VestuarioSaida> VestuarioSaidas { get; set; }

    #region
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // 🔹 Necessário para Identity funcionar

        modelBuilder.Entity<EfectivoOrdemServico>()
            .HasOne(e => e.Patente)
            .WithMany()
            .HasForeignKey(e => e.PatenteId)
            .OnDelete(DeleteBehavior.NoAction);

        // Relacionamento com Provincia de Nascimento
        modelBuilder.Entity<Efectivo>()
            .HasOne(c => c.ProvinciaNascimento)
            .WithMany()
            .HasForeignKey(c => c.ProvinciaNascId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento com Provincia de Nascimento
        modelBuilder.Entity<Efectivo>()
            .HasOne(c => c.ProvinciaResidencia)
            .WithMany()
            .HasForeignKey(c => c.ProvinciaResId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Dispositivo>()
     .HasOne(d => d.DispositivoTipo)
     .WithMany()
     .HasForeignKey(d => d.DispositivoTipoId)
     .OnDelete(DeleteBehavior.Restrict); // ou NoAction

        modelBuilder.Entity<Dispositivo>()
            .HasOne(d => d.DispositivoMarca)
            .WithMany()
            .HasForeignKey(d => d.DispositivoMarcaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Dispositivo>()
            .HasOne(d => d.DispositivoModelo)
            .WithMany()
            .HasForeignKey(d => d.DispositivoModeloId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    #endregion

}
