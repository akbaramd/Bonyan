using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.Security.Claims;

namespace Bonyan.IdentityManagement;

internal class DefaultClaimProvider<TUser> 
    : IBonIdentityClaimProvider<TUser> where TUser:IBonIdentityUser
{
private readonly IBonIdentityUserManager<TUser> _identityUserManager;

public DefaultClaimProvider(IBonIdentityUserManager<TUser> identityUserManager)
{
    _identityUserManager = identityUserManager;
}

public async Task<IEnumerable<Claim>> GenerateClaimsAsync(TUser user)
{
    
    var roles = await _identityUserManager.GetUserRolesAsync(user);
        var claims = new List<Claim>
        {
            new(BonClaimTypes.UserId, user.Id.Value.ToString()),
            new(BonClaimTypes.UserName, user.UserName),
            new(BonClaimTypes.Email, user.Email?.Address ?? string.Empty),
            new(BonClaimTypes.PhoneNumber, user.PhoneNumber?.Number ?? string.Empty),
            new(BonClaimTypes.RememberMe, "false") ,
            new(BonClaimTypes.Role, string.Join(",",roles.Value.Select(x=>x.Id.Value))) ,
        };

        return claims;
}


}