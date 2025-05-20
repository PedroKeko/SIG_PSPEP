namespace SIG_PSPEP.Areas.Dpq.Models
{
    public class EfetividadeLancamentoViewModel
    {
        public int EfectivoId { get; set; }
        public int EfectividadeTipoId { get; set; }
        public DateTime DataPresenca { get; set; } = DateTime.Now;
    }
}
