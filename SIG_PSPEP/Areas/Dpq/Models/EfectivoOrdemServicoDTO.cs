namespace SIG_PSPEP.Areas.Dpq.Models
{
    public class EfectivoOrdemServicoDTO
    {
        public int EfectivoId { get; set; }
        public int OrdemServicoId { get; set; }
        public int PatenteId { get; set; }
        public string? NumDespacho { get; set; }
    }
}
