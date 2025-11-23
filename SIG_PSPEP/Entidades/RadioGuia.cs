using SIG_PSPEP.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class RadioGuia : EntidadeBase
    {
        [StringLength(60)]
        public string? NumGuia { get; set; }
        public string? Observacao { get; set; }
        public bool Aprovado { get; set; } = false;
        public DateTime? DataAprovacao { get; set; }
        public string? ChefeId { get; set; }
    }
}
