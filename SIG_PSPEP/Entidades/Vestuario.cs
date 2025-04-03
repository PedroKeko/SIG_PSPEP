using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class Vestuario : EntidadeBase
    {

        [Required, MaxLength(250)]
        public string? Designação { get; set; }

        [Required, MaxLength(80)]
        public string? Classe { get; set; }
        public int Qtd { get; set; }

        [Required, MaxLength(50)]
        public string? EstadoVestuario { get; set; }

    }
}
