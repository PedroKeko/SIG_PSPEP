using SIG_PSPEP.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class MensagesAdmin 
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(250), Required(ErrorMessage = "O nome é obrigatório.")]
        public string? Nome { get; set; }

        [MaxLength(50)]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(20), Required(ErrorMessage = "O telefone é obrigatório.")]
        public string? Telefone { get; set; }

        [MaxLength(500), Required(ErrorMessage = "O mensagem é obrigatório.")]
        public string? Mensagem { get; set; }

        public DateTime DataRegisto { get; set; } = DateTime.Now;

        public bool Estado { get; set; } = true;

    }
}
