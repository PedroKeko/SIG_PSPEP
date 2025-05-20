using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class DispositivoTipo : EntidadeBase
    {
       
        [Required, MaxLength(250)]
        public string? TiposDispositivo { get; set; } //PC, Impressora, Tela...

    }
}
