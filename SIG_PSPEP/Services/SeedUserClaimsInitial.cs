using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace SIG_PSPEP.Services;

public class SeedUserClaimsInitial: ISeedUserClaimsInitial
{
    private readonly UserManager<IdentityUser> _userManager;
    public SeedUserClaimsInitial(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task SeedUserClaims()
    {
        try
        {
            //usuário 1
            IdentityUser user1 = await _userManager.FindByEmailAsync("gerente@localhost");
            if (user1 is not null)
            {
                var claimList = (await _userManager.GetClaimsAsync(user1))
                                                   .Select(p => p.Type);

                if (!claimList.Contains("CadastradoEm"))
                {
                    var claimResult1 = await _userManager.AddClaimAsync(user1,
                             new Claim("CadastradoEm", "03/03/2021"));
                }
            }

            // usuário 2
            IdentityUser user2 = await _userManager.FindByEmailAsync("usuario@localhost");
            if (user2 is not null)
            {
                var claimList = (await _userManager.GetClaimsAsync(user2))
                                                   .Select(p => p.Type);

                if (!claimList.Contains("CadastradoEm"))
                {
                    var claimResult1 = await _userManager.AddClaimAsync(user2,
                             new Claim("CadastradoEm", "01/01/2020"));
                }
            }
            //usuário 3
            IdentityUser user3 = await _userManager.FindByEmailAsync("macoratti@yahoo.com");
            if (user3 is not null)
            {
                var claimList = (await _userManager.GetClaimsAsync(user3))
                                                   .Select(p => p.Type);

                if (!claimList.Contains("CadastradoEm"))
                {
                    var claimResult1 = await _userManager.AddClaimAsync(user3,
                             new Claim("CadastradoEm", "02/02/2017"));
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
}
