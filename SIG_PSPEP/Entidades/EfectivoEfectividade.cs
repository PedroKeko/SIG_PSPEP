using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SIG_PSPEP.Enums;

namespace SIG_PSPEP.Entidades
{
    public class EfectivoEfectividade : EntidadeBase
    {
        public int EfectivoId { get; set; }
        public int EfectividadeTipoId { get; set; }
        public DateTime? DataPresenca { get; set; }
        public string? Justificacao { get; set; }
        public EfectividadeTipo? EfectividadeTipo { get; set; }
        public Efectivo? Efectivo { get; set; }

    }
}
