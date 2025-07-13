using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Users;

namespace Bonyan.IdentityManagement;

public interface IBonIdentityClaimProvider<TUser> where TUser : BonIdentityUser
{
    Task<IEnumerable<Claim>> GenerateClaimsAsync(TUser user);
}