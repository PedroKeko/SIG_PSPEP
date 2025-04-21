using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Areas.Admin.Models
{
    public class RegistroViewModel
    {
        public string? UserId { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "Telefone")]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "senha")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme a senha")]
        [Compare("Password", ErrorMessage = "As senhas não conferem")]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "Numero de Telefone")]
        public string? PhoneNamber { get; set; }

        public int AreaId { get; set; }
        public int EfectivoId { get; set; }
        public bool EstadoConta { get; set; }
    }
}
