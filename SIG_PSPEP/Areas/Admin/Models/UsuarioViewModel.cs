namespace SIG_PSPEP.Areas.Admin.Models
{
    public class UsuarioViewModel
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? NomeEfectivo { get; set; }
        public string? Area { get; set; }
        public List<string>? Roles { get; set; }
        public string? FotoBase64 { get; set; }
    }
}
