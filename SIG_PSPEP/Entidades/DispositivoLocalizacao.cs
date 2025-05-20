using SIG_PSPEP.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class DispositivoLocalizacao : EntidadeBase
    {
        public int DispositivoId { get; set; }
        public int OrgaoUnidadeId { get; set; }

        [StringLength(500)]
        public string? Obs { get; set; }
        public Dispositivo? Dispositivo { get; set; }
        public OrgaoUnidade? OrgaoUnidades { get; set; }
    }
}
