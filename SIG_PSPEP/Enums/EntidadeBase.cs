using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Projeto_ADREC.Enums
{
    public class EntidadeBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool Estado { get; set; } = true; // Define o Activo como true por padrão

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Registro")]
        public DateTime DataRegisto { get; set; } = DateTime.Now;

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Ultima Alteração")]
        public DateTime DataUltimaAlterecao { get; set; } = DateTime.Now;
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }
    }
}
