using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;

namespace Bonyan.IdentityManagement;

public interface IBonIdentityClaimProvider<TUser,TRole> where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
    Task<IEnumerable<Claim>> GenerateClaimsAsync(TUser user);
}