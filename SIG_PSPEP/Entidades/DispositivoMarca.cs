using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class DispositivoMarca : EntidadeBase
    {
       
        [Required, MaxLength(250)]
        public string? MarcasDispositivo { get; set; } //HP, IBM, Acer...

    }
}
