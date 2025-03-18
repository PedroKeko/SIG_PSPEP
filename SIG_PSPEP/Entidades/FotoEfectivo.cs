using SIG_PSPEP.Enums;

namespace SIG_PSPEP.Entidades
{
    public class FotoEfectivo: EntidadeBase
    {
        public int EfectivoId { get; set; }

        public byte[]? Foto { get; set; }

        public Efectivo? Efectivo { get; set; }
    }
}
