using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class UtencilioTipo : EntidadeBase
    {
        [Required, MaxLength(50)]
        public string? TipoUtencilio { get; set; }

    }
}
