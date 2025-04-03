using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class ArmamentoCondicaoTipo : EntidadeBase
    {

        [Required, MaxLength(250)]
        public string? TipoCondicao { get; set; }
    }
}
