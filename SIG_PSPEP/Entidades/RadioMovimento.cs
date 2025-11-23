using SIG_PSPEP.Enums;
using System.ComponentModel;

namespace SIG_PSPEP.Entidades
{
    public class RadioMovimento : EntidadeBase
    {
        public int RadioId { get; set; }
        public int RadioGuiaId { get; set; }
        public int OrgUnidPnaMinintId { get; set; }
        public string? TipoMovimento { get; set; }
        public Radio? Radio { get; set; }
        public RadioGuia? RadioGuia { get; set; }
        public OrgUnidPnaMinint? OrgUnidPnaMinint { get; set; }
    }

}
