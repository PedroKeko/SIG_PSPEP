namespace SIG_PSPEP.Areas.Dpq.Models
{
    public class EfectivoDiaStatus
    {
        public int EfectivoId { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Patente { get; set; } = string.Empty;
        public string NIP { get; set; } = string.Empty;

        // Dicionário: chave = dia do mês (1..31), valor = status ("Presente", "Ausente", "Dispensado", "Justificado", "")
        public Dictionary<int, string> StatusPorDia { get; set; } = new Dictionary<int, string>();

        // Estatísticas do mês
        public int QuantidadePresente { get; set; } = 0;
        public int QuantidadeAusente { get; set; } = 0;
        public int QuantidadeDispensado { get; set; } = 0;
        public int QuantidadeJustificado { get; set; } = 0;
    }
}
