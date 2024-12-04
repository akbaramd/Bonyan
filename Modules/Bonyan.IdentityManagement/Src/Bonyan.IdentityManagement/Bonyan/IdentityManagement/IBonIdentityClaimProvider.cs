using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Users;

namespace Bonyan.IdentityManagement;

public interface IBonIdentityClaimProvider<TUser> where TUser : IBonIdentityUser
{
    Task<IEnumerable<Claim>> GenerateClaimsAsync(TUser user);
}