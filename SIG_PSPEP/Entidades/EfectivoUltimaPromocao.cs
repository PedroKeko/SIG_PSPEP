using SIG_PSPEP.Enums;

namespace SIG_PSPEP.Entidades
{
    public class EfectivoUltimaPromocao : EntidadeBase
    {
        public int EfectivoId { get; set; }
        public int OrdemServicoId { get; set; }
        public int PatenteId { get; set; }
        public string? NumDespacho { get; set; }
        public OrdemServico? OrdemServico { get; set; }
        public Efectivo? Efectivo { get; set; }
        public Patente? Patente { get; set; }
    }
}
