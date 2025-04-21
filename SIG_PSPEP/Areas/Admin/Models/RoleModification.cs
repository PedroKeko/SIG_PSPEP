using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Areas.Admin.Models;

public class RoleModification
{
    [Display(Name = "Nome da Função")]
    public string? RoleName { get; set; }
    public string? RoleId { get; set; }
    public int UserCount { get; set; }
    public string[]? AddIds { get; set; }
    public string[]? DeleteIds { get; set; }
}
