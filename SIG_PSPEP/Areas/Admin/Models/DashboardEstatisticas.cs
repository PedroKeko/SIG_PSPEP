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
        public List<RegistroPorMes>? RegistrosPorMes { get; set; }
        public List<string>? UltimosUsuarios { get; set; }
        public List<string>? UltimosLogins { get; set; }
    }
    public class RegistroPorMes
    {
        public string AnoMes { get; set; } // Formato "Mês/Ano"
        public int Contagem { get; set; }  // Contagem de registros
    }
}
