using Microsoft.AspNetCore.Identity;
using SIG_PSPEP.Models;

namespace SIG_PSPEP.Areas.Admin.Models
{
    public class IndexViewModel
    {
        public IEnumerable<IdentityUser> Users { get; set; }
        public RegistroViewModel Registro { get; set; }
    }
}
