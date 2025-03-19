using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Models
{
    public class RegistroViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "Telefone")]
        public string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "senha")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme a senha")]
        [Compare("Password", ErrorMessage = "As senhas não conferem")]
        public string? ConfirmPassword { get; set; }

        public int UnidadeId { get; set; }
        public string? NIP { get; set; }
        public string? Patente { get; set; }

        public IFormFile? Foto { get; set; } // Mude para IFormFile para uploads de arquivos
    }
}
