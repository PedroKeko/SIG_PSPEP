using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class AlimentoControle : EntidadeBase
    {
        public int AlimentoId { get; set; }
        public int Qtd { get; set; }

        [Required, MaxLength(250)]
        public string? Designacao { get; set; }
        public DateTime DataExpiracao { get; set; }

        [Required, MaxLength(50)]
        public string? EstadoAlimento { get; set; }
        public Alimento? Alimento { get; set; }

    }
}
