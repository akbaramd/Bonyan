using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Users;

namespace Bonyan.IdentityManagement.ClaimProvider;

/// <summary>
/// Used by the Identity module to collect claims from all registered
/// <see cref="IBonIdentityClaimProvider"/> implementations. Builds a context from the domain user
/// and passes it to providers so they do not depend on the domain.
/// </summary>
public interface IBonIdentityClaimProviderManager
{
    /// <summary>
    /// Gets all claims for the given user by building a context and calling all registered claim providers.
    /// </summary>
    Task<IEnumerable<Claim>> GetClaimsAsync(BonIdentityUser user);
}
