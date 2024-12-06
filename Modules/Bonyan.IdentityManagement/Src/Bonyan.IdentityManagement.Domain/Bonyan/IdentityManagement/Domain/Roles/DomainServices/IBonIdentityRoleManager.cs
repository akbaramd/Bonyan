using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.DomainService;

namespace Bonyan.IdentityManagement.Domain.Roles.DomainServices
{
    /// <summary>
    /// Interface for managing identity roles and their permissions.
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
        /// Creates a role with associated permissions.
        /// </summary>
        /// <param name="role">The role to create.</param>
        /// <param name="permissionIds">The permissions to assign to the role.</param>
        /// <returns>A result indicating success or failure.</returns>
        Task<BonDomainResult> CreateRoleWithPermissionsAsync(BonIdentityRole role, IEnumerable<BonPermissionId> permissionIds);

        /// <summary>
        /// Assigns permissions to a role.
        /// </summary>
        /// <param name="role">The role to assign permissions to.</param>
        /// <param name="permissionIds">The permissions to assign.</param>
        /// <returns>A result indicating success or failure.</returns>
        Task<BonDomainResult> AssignPermissionsToRoleAsync(BonIdentityRole role, IEnumerable<BonPermissionId> permissionIds);

        /// <summary>
        /// Removes a permission from a role.
        /// </summary>
        /// <param name="role">The role to remove the permission from.</param>
        /// <param name="permissionId">The permission to remove.</param>
        /// <returns>A result indicating success or failure.</returns>
        Task<BonDomainResult> RemovePermissionFromRoleAsync(BonIdentityRole role, BonPermissionId permissionId);

        /// <summary>
        /// Finds a role by its ID.
        /// </summary>
        /// <param name="roleKey">The ID of the role.</param>
        /// <returns>A result containing the role or an error message.</returns>
        Task<BonDomainResult<BonIdentityRole>> FindRoleByIdAsync(string roleKey);
    }
}
