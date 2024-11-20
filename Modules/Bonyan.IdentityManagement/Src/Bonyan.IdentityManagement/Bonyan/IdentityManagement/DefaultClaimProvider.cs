using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Security.Claims;

namespace Bonyan.IdentityManagement;

internal class DefaultClaimProvider
    : IBonIdentityClaimProvider
{
    public Task<IEnumerable<Claim>> GenerateClaimsAsync(IBonIdentityUser user)
    {
        var claims = new List<Claim>
        {
            new(BonClaimTypes.UserId, user.Id.Value.ToString()),
            new(BonClaimTypes.UserName, user.UserName),
            new(BonClaimTypes.Email, user.Email?.Address ?? string.Empty),
            new(BonClaimTypes.PhoneNumber, user.PhoneNumber?.Number ?? string.Empty),
            new(BonClaimTypes.RememberMe, "false") // Default value
        };

        return Task.FromResult<IEnumerable<Claim>>(claims);
    }
}