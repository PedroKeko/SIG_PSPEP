using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Entidades
{
   
    public class LogsEvento
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public string? TipoEvento { get; set; }
        public string? Obs { get; set; }
        public DateTime DataRegisto { get; set; }
        public virtual IdentityUser? User { get; set; }
    }
}
