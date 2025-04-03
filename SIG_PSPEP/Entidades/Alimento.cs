using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class Alimento : EntidadeBase
    {
        public int AlimentoCategoriaId { get; set; }
       
        [Required, MaxLength(250)]
        public string? Designacao { get; set; }
        public AlimentoCategoria? AlimentoCategoria { get; set; }

    }
}
