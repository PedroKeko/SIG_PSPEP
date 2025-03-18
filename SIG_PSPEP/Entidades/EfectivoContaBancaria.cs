using SIG_PSPEP.Enums;

namespace SIG_PSPEP.Entidades
{
    public class EfectivoContaBancaria : EntidadeBase
    {
        public int EfectivoId { get; set; }
        public int BancoId { get; set; }
        public string? NumeroConta { get; set; }
        public string? IBAN { get; set; }
        public Efectivo? Efectivo { get; set; }
        public Banco? Banco{ get; set; }
    }
}
