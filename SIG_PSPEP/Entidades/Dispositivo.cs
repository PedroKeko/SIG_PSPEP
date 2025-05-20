using SIG_PSPEP.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class Dispositivo : EntidadeBase
    {
        public int DispositivoTipoId { get; set; }
        public int DispositivoMarcaId { get; set; }
        public int DispositivoModeloId { get; set; }

        [StringLength(20)]
        public string? Cod_Dispositivo { get; set; }

        [StringLength(100), DisplayName("Sistema Operativo")]
        public string? SO { get; set; }

        [StringLength(50)]
        public string? MAC { get; set; }

        [StringLength(30)]
        public string? RAM { get; set; }

        [StringLength(30)]
        public string? ROM { get; set; }

        [StringLength(100)]
        public string? PROCESSADOR { get; set; }
        public DispositivoTipo? DispositivoTipo { get; set; }
        public DispositivoMarca? DispositivoMarca { get; set; }
        public DispositivoModelo? DispositivoModelo { get; set; }
    }
}
