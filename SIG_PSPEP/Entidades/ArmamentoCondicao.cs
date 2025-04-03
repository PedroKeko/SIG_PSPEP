using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class ArmamentoCondicao : EntidadeBase
    {
        public int EfectivoId { get; set; }
        public int ArmamentoCondicaoTipoId { get; set; }

        [Required, MaxLength(250)]
        public string? Descricao { get; set; }

        [Required, MaxLength(250)]
        public string? OBS { get; set; }
        public Efectivo? Efectivo { get; set; }
        public ArmamentoCondicaoTipo? ArmamentoCondicaoTipo { get; set; }
    }
}
