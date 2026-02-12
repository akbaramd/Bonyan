using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.DomainService;

namespace Bonyan.IdentityManagement.Domain.Roles.DomainServices;

/// <summary>
/// Domain service for identity roles (non-generic).
/// </summary>
public interface IBonIdentityRoleManager
{
    Task<BonDomainResult> CreateRoleAsync(BonIdentityRole role);
    Task<BonDomainResult> UpdateRoleAsync(BonIdentityRole role);
    Task<BonDomainResult> DeleteRoleAsync(BonIdentityRole role);
    Task<BonDomainResult> CreateRoleWithClaimsAsync(BonIdentityRole role, IEnumerable<(string claimType, string claimValue, string? issuer)> claims);
    Task<BonDomainResult> AddClaimToRoleAsync(BonIdentityRole role, string claimType, string claimValue, string? issuer = null);
    Task<BonDomainResult> RemoveClaimFromRoleAsync(BonIdentityRole role, string claimType, string claimValue);
    Task<BonDomainResult> RemoveClaimsByTypeFromRoleAsync(BonIdentityRole role, string claimType);
    Task<BonDomainResult<bool>> HasClaimAsync(BonIdentityRole role, string claimType, string claimValue);
    Task<BonDomainResult<IEnumerable<BonIdentityRoleClaims>>> GetClaimsByTypeAsync(BonIdentityRole role, string claimType);
    Task<BonDomainResult<BonIdentityRole>> FindRoleByIdAsync(string roleKey);
    Task<BonDomainResult<IEnumerable<BonIdentityRole>>> GetAllRolesAsync();
}
