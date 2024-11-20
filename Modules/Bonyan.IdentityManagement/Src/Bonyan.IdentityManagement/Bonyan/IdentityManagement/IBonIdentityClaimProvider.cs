using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Users;

namespace Bonyan.IdentityManagement;

public interface IBonIdentityClaimProvider
{
    Task<IEnumerable<Claim>> GenerateClaimsAsync(IBonIdentityUser user);
}