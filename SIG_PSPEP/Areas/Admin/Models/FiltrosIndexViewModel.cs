using Microsoft.AspNetCore.Identity;

namespace SIG_PSPEP.Areas.Admin.Models
{
    public class FiltrosIndexViewModel
    {
    }
    public class RolesIndexViewModel
    {
        public List<IdentityRole> PaginacaoRoles { get; set; }
        public string SearchString { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
    }
}
