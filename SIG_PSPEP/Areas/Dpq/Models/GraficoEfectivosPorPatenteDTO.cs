namespace SIG_PSPEP.Areas.Dpq.Models
{
    public class GraficoEfectivosPorPatenteDTO
    {
        public string? Patente { get; set; }
        public Dictionary<string, int> QuantidadePorOrgaoUnidade { get; set; } = new();
        public Dictionary<string, double> PercentagemPorOrgaoUnidade { get; set; } = new();
    }
}
