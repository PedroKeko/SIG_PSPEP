using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class VestuarioSaida : EntidadeBase
    {

        public int VestuarioId { get; set; }
        public int EfectivoId { get; set; }

        [Required]
        public int Qtd { get; set; }

        [Required, MaxLength(250)]
        public string? OBS { get; set; }

        public Vestuario? Vestuario { get; set; }
        public Efectivo? Efectivo { get; set; }

    }
}
