using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SIG_PSPEP.Enums;

namespace SIG_PSPEP.Entidades
{
    public class RadioOrgaoUnidade : EntidadeBase
    {
        public int RadioId { get; set; }
        public int OrgaoUnidadeId { get; set; }
        public Radio? Radio { get; set; }
        public OrgaoUnidade? OrgaoUnidade { get; set; }
    }
}
