using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class ArmamentoLocalizacao : EntidadeBase
    {
        public int ArmamentoId { get; set; }
        public int UnidadeId { get; set; }

        [Required, MaxLength(50)]
        public string? NumeroArma { get; set; }

        [Required, MaxLength(50)]
        public string? EstadoArma { get; set; }

        [MaxLength(250)]
        public string? OBS { get; set; }
        public Armamento? Armamento { get; set; }
        public OrgaoUnidade? OrgaoUnidade { get; set; }
    }
}
