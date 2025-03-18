using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class Filiacao : EntidadeBase
    {
        public int EfectivoId { get; set; }
        public string? Pai { get; set; }
        public string? ApelidoPai { get; set; }
        public string? NacionalidadePai { get; set; }
        public string? NaturalidadePai { get; set; }
        public string? Mae { get; set; }
        public string? ApelidoMae { get; set; }
        public string? NacionalidadeMae { get; set; }
        public string? NaturalidadeMae { get; set; }
        public Efectivo? Efectivo { get; set; }
    }
}
