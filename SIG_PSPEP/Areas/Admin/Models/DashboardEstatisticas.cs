using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Admin.Models
{
    public class DashboardEstatisticas
    {
        public int TotalUsuarios { get; set; }
        public int TotalRoles { get; set; }
        public int TotalAreas { get; set; }
        public int TotalClaims { get; set; }
        public int UsuariosAtivos { get; set; }
        public int UsuariosDesativados { get; set; }
        public int OrgaoUnidades { get; set; }
        public int Provincias { get; set; }
        public int Municipios { get; set; }
        public int LogsEventos { get; set; }
        public int LogsAcessos { get; set; }
        public int Bancos { get; set; }
        public List<RegistroPorMes>? RegistrosPorMes { get; set; }
        public List<string>? UltimosUsuarios { get; set; }
        public List<LogsAcessoViewModel>? UltimosLogins { get; set; }
    }
    public class RegistroPorMes
    {
        public string AnoMes { get; set; } // Formato "Mês/Ano"
        public int Contagem { get; set; }  // Contagem de registros
    }

    public class LogsAcessoViewModel
    {
        public string? Email { get; set; }
        public string? TipoAcesso { get; set; }
        public string? Obs { get; set; }
        public DateTime DataRegisto { get; set; }
    }
}
