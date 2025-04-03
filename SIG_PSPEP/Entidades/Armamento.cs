using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class Armamento : EntidadeBase
    {

        [Required, MaxLength(250)]
        public string? Marca { get; set; }

        [Required, MaxLength(250)]
        public string? Modelo { get; set; }

        [MaxLength(20)]
        public string? Calibre { get; set; }

        [Required, MaxLength(100)]
        public string? Categoria { get; set; }

        [MaxLength(100)]
        public string? Origem { get; set; }

    }
}
