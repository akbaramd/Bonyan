using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;

namespace Bonyan.IdentityManagement;

public interface IBonIdentityClaimProviderManager<TUser,TRole> where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
    /// <summary>
    /// Aggregates claims from all registered providers.
    /// </summary>
    /// <param name="user">The user for whom claims are generated.</param>
    /// <returns>A list of aggregated claims.</returns>
    Task<IEnumerable<Claim>> GetClaimsAsync(TUser user);
}