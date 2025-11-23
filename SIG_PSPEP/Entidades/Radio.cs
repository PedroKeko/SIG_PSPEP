using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SIG_PSPEP.Enums;

namespace SIG_PSPEP.Entidades
{
    public class Radio : EntidadeBase
    {
        public int RadioTipoId { get; set; }
        [StringLength(20)]
        public string? CodRadio { get; set; }

        [StringLength(50)]
        public string? IdRadio { get; set; }

        [StringLength(50)]
        public string? TEI { get; set; }

        [StringLength(50)]
        public string? NumSerie { get; set; }

        [StringLength(80)]
        public string? EstadoTecnico { get; set; }
        public RadioTipo? RadioTipo { get; set; }
    }
}
