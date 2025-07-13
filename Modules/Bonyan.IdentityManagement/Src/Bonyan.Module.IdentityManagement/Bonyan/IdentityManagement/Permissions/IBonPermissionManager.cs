using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Permissions;

/// <summary>
/// Interface for managing permissions in the system
/// </summary>
public interface IBonPermissionManager
{
    /// <summary>
    /// Initializes all permissions from registered providers
    /// </summary>
    void InitializePermissions();

    /// <summary>
    /// Gets all defined permissions
    /// </summary>
    /// <returns>Collection of all permissions</returns>
    IEnumerable<PermissionDefinition> GetAllPermissions();

    /// <summary>
    /// Gets all permission groups
    /// </summary>
    /// <returns>Collection of all permission groups</returns>
    IEnumerable<PermissionGroupDefinition> GetAllGroups();

    /// <summary>
    /// Gets a permission by name
    /// </summary>
    /// <param name="name">Permission name</param>
    /// <returns>Permission definition or null if not found</returns>
    PermissionDefinition? GetPermission(string name);

    /// <summary>
    /// Gets a permission group by name
    /// </summary>
    /// <param name="name">Group name</param>
    /// <returns>Permission group definition or null if not found</returns>
    PermissionGroupDefinition? GetGroup(string name);

    /// <summary>
    /// Checks if a permission exists
    /// </summary>
    /// <param name="name">Permission name</param>
    /// <returns>True if permission exists</returns>
    bool PermissionExists(string name);

    /// <summary>
    /// Gets permissions for a specific group
    /// </summary>
    /// <param name="groupName">Group name</param>
    /// <returns>Permissions in the group</returns>
    IEnumerable<PermissionDefinition> GetPermissionsForGroup(string groupName);

    /// <summary>
    /// Gets child permissions for a parent permission
    /// </summary>
    /// <param name="parentPermissionName">Parent permission name</param>
    /// <returns>Child permissions</returns>
    IEnumerable<PermissionDefinition> GetChildPermissions(string parentPermissionName);

    /// <summary>
    /// Grants a permission to a user via claims
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="permission">Permission name</param>
    /// <param name="issuer">Optional issuer</param>
    /// <returns>Task representing the operation</returns>
    Task GrantPermissionToUserAsync(BonUserId userId, string permission, string? issuer = null);

    /// <summary>
    /// Revokes a permission from a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="permission">Permission name</param>
    /// <returns>Task representing the operation</returns>
    Task RevokePermissionFromUserAsync(BonUserId userId, string permission);

    /// <summary>
    /// Grants a permission to a role via claims
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permission">Permission name</param>
    /// <param name="issuer">Optional issuer</param>
    /// <returns>Task representing the operation</returns>
    Task GrantPermissionToRoleAsync(BonRoleId roleId, string permission, string? issuer = null);

    /// <summary>
    /// Revokes a permission from a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permission">Permission name</param>
    /// <returns>Task representing the operation</returns>
    Task RevokePermissionFromRoleAsync(BonRoleId roleId, string permission);

    /// <summary>
    /// Gets all permissions granted to a user (direct and through roles)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User permissions</returns>
    Task<IEnumerable<string>> GetUserPermissionsAsync(BonUserId userId);

    /// <summary>
    /// Gets all permissions granted to a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>Role permissions</returns>
    Task<IEnumerable<string>> GetRolePermissionsAsync(BonRoleId roleId);

    /// <summary>
    /// Checks if a user has a specific permission
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="permission">Permission name</param>
    /// <returns>True if user has permission</returns>
    Task<bool> HasPermissionAsync(BonUserId userId, string permission);
} 