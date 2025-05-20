using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class EfectividadeTipo 
    {
        public int Id { get; set; }
       
        [Required, MaxLength(250)]
        public string? DescTipo { get; set; }
        public string? Sigla { get; set; }

    }
}
