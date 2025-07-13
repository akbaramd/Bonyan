using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.User;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Bonyan.IdentityManagement.Permissions;

/// <summary>
/// Interface for checking permissions in application services
/// </summary>
public interface IBonPermissionChecker
{
    /// <summary>
    /// Checks if the current user has a specific permission
    /// </summary>
    /// <param name="permission">Permission name to check</param>
    /// <returns>True if the current user has the permission</returns>
    Task<bool> IsGrantedAsync(string permission);

    /// <summary>
    /// Checks if a specific user has a permission
    /// </summary>
    /// <param name="userId">User ID to check</param>
    /// <param name="permission">Permission name to check</param>
    /// <returns>True if the user has the permission</returns>
    Task<bool> IsGrantedAsync(BonUserId userId, string permission);

    /// <summary>
    /// Checks if the current user has any of the specified permissions
    /// </summary>
    /// <param name="permissions">Permission names to check</param>
    /// <returns>True if the current user has any of the permissions</returns>
    Task<bool> IsGrantedAnyAsync(params string[] permissions);

    /// <summary>
    /// Checks if the current user has all of the specified permissions
    /// </summary>
    /// <param name="permissions">Permission names to check</param>
    /// <returns>True if the current user has all of the permissions</returns>
    Task<bool> IsGrantedAllAsync(params string[] permissions);

    /// <summary>
    /// Checks if a specific user has any of the specified permissions
    /// </summary>
    /// <param name="userId">User ID to check</param>
    /// <param name="permissions">Permission names to check</param>
    /// <returns>True if the user has any of the permissions</returns>
    Task<bool> IsGrantedAnyAsync(BonUserId userId, params string[] permissions);

    /// <summary>
    /// Checks if a specific user has all of the specified permissions
    /// </summary>
    /// <param name="userId">User ID to check</param>
    /// <param name="permissions">Permission names to check</param>
    /// <returns>True if the user has all of the permissions</returns>
    Task<bool> IsGrantedAllAsync(BonUserId userId, params string[] permissions);

    /// <summary>
    /// Gets all permissions for the current user
    /// </summary>
    /// <returns>Collection of permission names</returns>
    Task<IEnumerable<string>> GetGrantedPermissionsAsync();

    /// <summary>
    /// Gets all permissions for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Collection of permission names</returns>
    Task<IEnumerable<string>> GetGrantedPermissionsAsync(BonUserId userId);
}

/// <summary>
/// Implementation of permission checker
/// </summary>
public class BonPermissionChecker : IBonPermissionChecker
{
    private readonly IBonPermissionManager _permissionManager;
    private readonly IBonCurrentUser _bonCurrentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BonPermissionChecker(
        IBonPermissionManager permissionManager,
        IHttpContextAccessor httpContextAccessor, IBonCurrentUser bonCurrentUser)
    {
        _permissionManager = permissionManager;
        _httpContextAccessor = httpContextAccessor;
        _bonCurrentUser = bonCurrentUser;
    }

    public async Task<bool> IsGrantedAsync(string permission)
    {
        var userId = _bonCurrentUser.Id ;
        if (userId is null )
            return false;

        return await _permissionManager.HasPermissionAsync(BonUserId.NewId(userId.Value), permission);
    }

    public async Task<bool> IsGrantedAsync(BonUserId userId, string permission)
    {
        return await _permissionManager.HasPermissionAsync(userId, permission);
    }

    public async Task<bool> IsGrantedAnyAsync(params string[] permissions)
    {
        var userId = _bonCurrentUser.Id ;
        if (userId is null )
            return false;

        return await IsGrantedAnyAsync(BonUserId.NewId(userId.Value), permissions);
    }

    public async Task<bool> IsGrantedAllAsync(params string[] permissions)
    {
        var userId = _bonCurrentUser.Id ;
        if (userId is null )
            return false;

        return await IsGrantedAllAsync(BonUserId.NewId(userId.Value), permissions);
    }

    public async Task<bool> IsGrantedAnyAsync(BonUserId userId, params string[] permissions)
    {
        foreach (var permission in permissions)
        {
            if (await _permissionManager.HasPermissionAsync(userId, permission))
                return true;
        }
        return false;
    }

    public async Task<bool> IsGrantedAllAsync(BonUserId userId, params string[] permissions)
    {
        foreach (var permission in permissions)
        {
            if (!await _permissionManager.HasPermissionAsync(userId, permission))
                return false;
        }
        return true;
    }

    public async Task<IEnumerable<string>> GetGrantedPermissionsAsync()
    {
        var userId = _bonCurrentUser.Id ;
        if (userId is null )
            return Enumerable.Empty<string>();

        return await _permissionManager.GetUserPermissionsAsync(BonUserId.NewId(userId.Value));
    }

    public async Task<IEnumerable<string>> GetGrantedPermissionsAsync(BonUserId userId)
    {
        return await _permissionManager.GetUserPermissionsAsync(userId);
    }

   
} 