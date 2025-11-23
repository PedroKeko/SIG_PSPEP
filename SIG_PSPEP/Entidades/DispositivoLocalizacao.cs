using SIG_PSPEP.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class DispositivoLocalizacao : EntidadeBase
    {
        public int DispositivoId { get; set; }
        public int OrgUnidPnaMinintId { get; set; }

        [StringLength(500)]
        public string? Obs { get; set; }
        public string? EstadoTecnico { get; set; }
        public Dispositivo? Dispositivo { get; set; }
        public OrgUnidPnaMinint? OrgUnidPnaMinint { get; set; }
    }
}
