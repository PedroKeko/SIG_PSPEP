using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Models
{
    public class PerfilUsuarioViewModel
    {
        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "Telefone")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Nome Completo")]
        public string? NomeCompleto { get; set; }

        public byte[]? Foto { get; set; }

        public string? Role { get; set; }

        public List<string> Roles { get; set; } = new();

        public List<string> Claims { get; set; } = new();

        [Required(ErrorMessage = "A senha atual é obrigatória.")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        [StringLength(100, ErrorMessage = "A {0} deve ter no mínimo {2} e no máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "As senhas não coincidem.")]
        public string? ConfirmNewPassword { get; set; }
    }
}


