using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class FuncaoCargo: EntidadeBase
    {
        [MaxLength(100)]
        public string? NomeFuncaoCargo { get; set; }
    }
}
