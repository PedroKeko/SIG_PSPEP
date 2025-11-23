//namespace SIG_PSPEP.Models;
//using System.ComponentModel.DataAnnotations;

//public class ForgotPasswordViewModel
//{
//    [Required]
//    [EmailAddress]
//    [Display(Name = "Email")]
//    public string Email { get; set; }
//}

//public class ResetPasswordViewModel
//{
//    [Required]
//    [EmailAddress]
//    [Display(Name = "Email")]
//    public string Email { get; set; }

//    [Required]
//    [StringLength(100, ErrorMessage = "A {0} deve ter ao menos {2} caracteres.", MinimumLength = 6)]
//    [DataType(DataType.Password)]
//    [Display(Name = "Nova Senha")]
//    public string Password { get; set; }

//    [DataType(DataType.Password)]
//    [Display(Name = "Confirmar Nova Senha")]
//    [Compare("Password", ErrorMessage = "As senhas não coincidem.")]
//    public string ConfirmPassword { get; set; }

//    public string Token { get; set; }
//}

