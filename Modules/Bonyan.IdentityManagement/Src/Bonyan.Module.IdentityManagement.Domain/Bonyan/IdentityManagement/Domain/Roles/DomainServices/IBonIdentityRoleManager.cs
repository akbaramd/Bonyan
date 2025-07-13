using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.DomainService;

namespace Bonyan.IdentityManagement.Domain.Roles.DomainServices
{
    /// <summary>
    /// Interface for managing identity roles and their claims.
    /// </summary>
    public interface IBonIdentityRoleManager
    {
        /// <summary>
        /// Creates a new role.
        /// </summary>
        /// <param name="role">The role to create.</param>
        /// <returns>A result indicating success or failure.</returns>
        Task<BonDomainResult> CreateRoleAsync(BonIdentityRole role);

        /// <summary>
        /// Updates an existing role.
        /// </summary>
        /// <param name="role">The role to update.</param>
        /// <returns>A result indicating success or failure.</returns>
        Task<BonDomainResult> UpdateRoleAsync(BonIdentityRole role);

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="role">The role to delete.</param>
        /// <returns>A result indicating success or failure.</returns>
        Task<BonDomainResult> DeleteRoleAsync(BonIdentityRole role);

        /// <summary>
        /// Creates a role with associated claims.
        /// </summary>
        /// <param name="role">The role to create.</param>
        /// <param name="claims">The claims to assign to the role.</param>
        /// <returns>A result indicating success or failure.</returns>
        Task<BonDomainResult> CreateRoleWithClaimsAsync(BonIdentityRole role, IEnumerable<(string claimType, string claimValue, string? issuer)> claims);

        /// <summary>
        /// Adds a claim to a role.
        /// </summary>
        /// <param name="role">The role to add the claim to.</param>
        /// <param name="claimType">The type of the claim.</param>
        /// <param name="claimValue">The value of the claim.</param>
        /// <param name="issuer">The issuer of the claim (optional).</param>
        /// <returns>A result indicating success or failure.</returns>
        Task<BonDomainResult> AddClaimToRoleAsync(BonIdentityRole role, string claimType, string claimValue, string? issuer = null);

        /// <summary>
        /// Removes a claim from a role.
        /// </summary>
        /// <param name="role">The role to remove the claim from.</param>
        /// <param name="claimType">The type of the claim to remove.</param>
        /// <param name="claimValue">The value of the claim to remove.</param>
        /// <returns>A result indicating success or failure.</returns>
        Task<BonDomainResult> RemoveClaimFromRoleAsync(BonIdentityRole role, string claimType, string claimValue);

        /// <summary>
        /// Removes all claims of a specific type from a role.
        /// </summary>
        /// <param name="role">The role to remove claims from.</param>
        /// <param name="claimType">The type of claims to remove.</param>
        /// <returns>A result indicating success or failure.</returns>
        Task<BonDomainResult> RemoveClaimsByTypeFromRoleAsync(BonIdentityRole role, string claimType);

        /// <summary>
        /// Checks if a role has a specific claim.
        /// </summary>
        /// <param name="role">The role to check.</param>
        /// <param name="claimType">The type of the claim.</param>
        /// <param name="claimValue">The value of the claim.</param>
        /// <returns>A result containing whether the role has the claim.</returns>
        Task<BonDomainResult<bool>> HasClaimAsync(BonIdentityRole role, string claimType, string claimValue);

        /// <summary>
        /// Gets all claims of a specific type for a role.
        /// </summary>
        /// <param name="role">The role to get claims for.</param>
        /// <param name="claimType">The type of claims to retrieve.</param>
        /// <returns>A result containing the claims.</returns>
        Task<BonDomainResult<IEnumerable<BonIdentityRoleClaims>>> GetClaimsByTypeAsync(BonIdentityRole role, string claimType);

        /// <summary>
        /// Finds a role by its ID.
        /// </summary>
        /// <param name="roleKey">The ID of the role.</param>
        /// <returns>A result containing the role or an error message.</returns>
        Task<BonDomainResult<BonIdentityRole>> FindRoleByIdAsync(string roleKey);
    }
}
