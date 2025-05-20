using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class DispositivoModelo : EntidadeBase
    {
        public int DispositivoMarcaId { get; set; }

        [Required, MaxLength(250)]
        public string? Modelo { get; set; } //vostro330, Deskjet 134...
        public DispositivoMarca? DispositivoMarca { get; set; }

    }
}
