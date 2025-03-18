using SIG_PSPEP.Enums;

namespace SIG_PSPEP.Entidades
{
    public class EfectivoOrdemServico : EntidadeBase
    {
        public int EfectivoId { get; set; }
        public int OrdemServicoId { get; set; }
        public int PatenteId { get; set; }
        public string? NumDespacho { get; set; }
        public DateTime AnoPromocao { get; set; }
        public EstadoEfectividade? EstadoEfectividade { get; set; }
        public Efectivo? Efectivo { get; set; }
        public Patente? Patente { get; set; }
    }
}
