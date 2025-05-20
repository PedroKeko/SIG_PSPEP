using System.ComponentModel.DataAnnotations;
using SIG_PSPEP.Enums;

namespace SIG_PSPEP.Entidades
{
    public class RadioTipo : EntidadeBase
    {
        [StringLength(60)]
        public string? Marca { get; set; }

        [StringLength(50)]
        public string? Modelo { get; set; }
    }
}
