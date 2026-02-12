using System.Security.Claims;

namespace Bonyan.IdentityManagement.ClaimProvider;

/// <summary>
/// Implemented by the Identity module and by other modules to add claims when building the user principal.
/// Receives <see cref="BonIdentityClaimProviderContext"/> only (no domain entity), so other modules
/// can contribute claims without referencing the Identity domain. Register implementations in DI;
/// <see cref="IBonIdentityClaimProviderManager"/> aggregates all providers.
/// </summary>
public interface IBonIdentityClaimProvider
{
    /// <summary>
    /// Generates claims for the user described by <paramref name="context"/>.
    /// </summary>
    Task<IEnumerable<Claim>> GenerateClaimsAsync(BonIdentityClaimProviderContext context);
}
