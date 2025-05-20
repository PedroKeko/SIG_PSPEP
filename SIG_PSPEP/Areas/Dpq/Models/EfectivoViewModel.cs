using SIG_PSPEP.Entidades;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIG_PSPEP.Areas.Dpq.Models
{
    public class EfectivoViewModel
    {
        public int Id { get; set; }
       
        public int SituacaoEfectivoId { get; set; }
        public int OrgaoUnidadeId { get; set; }
        public int FuncaoCargoId { get; set; }
        public int PatenteId { get; set; }
        public int MunicipioId { get; set; }
        public int ProvinciaNascId { get; set; }
        public int ProvinciaResId { get; set; }

        [Required, MaxLength(50)]
        public string? Num_Processo { get; set; }

        [Required, MaxLength(20)]
        public string? NIP { get; set; }
        [MaxLength(20)]
        public string? N_Agente { get; set; }

        [Required]
        [DisplayName("Nome Completo")]
        [StringLength(300)]
        public string? NomeCompleto { get; set; }

        [MaxLength(100)]
        public string? Apelido { get; set; }

        [Required, MaxLength(10)]
        public string? Genero { get; set; }

        public DateTime DataNasc { get; set; } = DateTime.Today;

        [MaxLength(20)]
        public string? EstadoCivil { get; set; }

        [MaxLength(5)]
        public string? GSanguineo { get; set; }

        [Required, MaxLength(20)]
        public string? NumBI { get; set; }

        public DateTime BIEmitido { get; set; } = DateTime.Today;
        public DateTime BIValidade { get; set; } = DateTime.Today.AddYears(10);

        [MaxLength(20)]
        public string? NumCartaConducao { get; set; }

        public DateTime CartaValidade { get; set; }
        public DateTime CartaEmitido { get; set; }

        [MaxLength(20)]
        public string? NumPassaporte { get; set; }

        public DateTime PassapValidade { get; set; }
        public DateTime PassapEmitido { get; set; }

        [Required, MaxLength(50)]
        public string? Nacionalidade { get; set; }

        [MaxLength(100)]
        public string? Destrito_BairroRes { get; set; }

        [MaxLength(100)]
        public string? Rua { get; set; }

        [MaxLength(10)]
        public string? CasaNum { get; set; }

        [MaxLength(50)]
        public string? Habilitacao { get; set; }

        [MaxLength(100)]
        public string? CursoHabilitado { get; set; }

        [MaxLength(100)]
        public string? InstitAcademica { get; set; }

        [MaxLength(20)]
        public string? Telefone1 { get; set; }

        [MaxLength(20)]
        public string? Telefone2 { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        public DateTime DataIngresso { get; set; } = DateTime.Today;

        [MaxLength(50)]
        public string? TipoVinculo { get; set; }

        [MaxLength(50)]
        public string? Carreira { get; set; }

        [MaxLength(100)]
        public string? UnidadeOrigem { get; set; }

        [MaxLength(255)]
        public string? OutrasInfo { get; set; }

        [DisplayName("Usuário")]
        public string? UserId { get; set; }

        // Propriedades para FotoCondutor
        [NotMapped]
        public IFormFile? FotoIF { get; set; }
        public  byte[]? FotoByte { get; set; }
        public int EfectivoId { get; set; }
    }
}
