using SIG_PSPEP.Enums;

namespace SIG_PSPEP.Entidades
{
    public class Patente : EntidadeBase
    {
        public string? Posto { get; set; }
        public string? Classe { get; set; }
        public int grau { get; set; }
        public int Diuturnidade { get; set; }
    }
}
