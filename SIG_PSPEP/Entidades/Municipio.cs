using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidade;

public class Municipio : EntidadeBase
{

    [Required]
    [StringLength(100)]
    public string? Nome { get; set; }
    public int ProvinciaId { get; set; }
    public Provincia? Provincia { get; set; }
}
