using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Entidades
{
   
    public class UsuarioAute
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public int AreaId { get; set; }
        public int EfectivoId { get; set; }
        public virtual IdentityUser? User { get; set; }
        public Area? Area { get; set; }
        public Efectivo? Efectivo { get; set; }
    }
}
