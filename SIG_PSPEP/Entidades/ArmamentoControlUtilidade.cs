using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class ArmamentoControlUtilidade : EntidadeBase
    {
        public int ArmamentoLocalizacaoId { get; set; }
        public int EfectivoId { get; set; }

        [Required, MaxLength(50)]
        public string? EstadoResponsabilidade { get; set; }
        public DateTime DataEntregua { get; set; }
        public DateTime DataDevolucao { get; set; }

        [MaxLength(250)]
        public string? MotivoEntrega { get; set; }
        public ArmamentoLocalizacao? ArmamentoLocalizacao { get; set; }
        public Efectivo? Efectivo { get; set; }
    }
}
