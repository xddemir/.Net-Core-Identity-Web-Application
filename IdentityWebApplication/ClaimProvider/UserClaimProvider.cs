using System.Security.Claims;
using IdentityWebApplication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace IdentityWebApplication.ClaimProvider;

public class UserClaimProvider : IClaimsTransformation
{
    private readonly UserManager<AppUser> _userManager;

    public UserClaimProvider(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identityPrinciple = principal.Identity as ClaimsIdentity;

        var currentUser = await _userManager.FindByNameAsync(identityPrinciple!.Name!);

        if (currentUser == null || currentUser!.City == null)
        {
            return principal;
        }

        if (principal.HasClaim(x => x.Type != "city"))
        {
            Claim cityClaim = new Claim("city", currentUser.City);

            identityPrinciple.AddClaim(cityClaim);
        }

        return principal;
    }
}