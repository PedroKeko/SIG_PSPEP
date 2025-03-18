using SIG_PSPEP.Enums;

namespace SIG_PSPEP.Entidades
{
    public class AgregadoFam : EntidadeBase
    {
        public int EfectivoId { get; set; }
        public string? NomeAgregado { get; set; }
        public string? GrauParentesco { get; set; }
        public DateTime DataNasc { get; set; }
        public Efectivo? Efectivo { get; set; }
    }
}
