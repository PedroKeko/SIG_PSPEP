using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class Utencilio : EntidadeBase
    {
        public int UtencilioTipoId { get; set; }
       
        [Required, MaxLength(250)]
        public string? Designacao { get; set; }
        public int Qtd { get; set; }

        [Required, MaxLength(250)]
        public string? OBS { get; set; }

        [Required, MaxLength(80)]
        public string? EstadoUtencilio { get; set; }
        public UtencilioTipo? UtencilioTipo { get; set; }

    }
}
