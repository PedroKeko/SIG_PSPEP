using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class VestuarioEntrada : EntidadeBase
    {
        public int VestuarioId { get; set; }

        [Required]
        public int Qtd { get; set; }

        [Required, MaxLength(250)]
        public string? OBS { get; set; }

        public Vestuario? Vestuario { get; set; }

    }
}
