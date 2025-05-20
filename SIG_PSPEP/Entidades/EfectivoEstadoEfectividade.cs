using SIG_PSPEP.Enums;

namespace SIG_PSPEP.Entidades
{
    public class EfectivoEstadoEfectividade : EntidadeBase
    {
        public int EfectivoId { get; set; }
        public int EstadoEfectividadeId { get; set; }
        public string? Observacao { get; set; }
        public EstadoEfectividade? EstadoEfectividade { get; set; }
        public Efectivo? Efectivo { get; set; }
    }
}
