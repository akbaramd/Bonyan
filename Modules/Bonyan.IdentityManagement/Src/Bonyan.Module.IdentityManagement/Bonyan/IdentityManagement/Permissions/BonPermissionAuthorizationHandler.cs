using System.Security.Claims;
using Bonyan.User;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.Permissions;

/// <summary>
/// Authorization handler for permission-based authorization using claims
/// </summary>
public class BonPermissionAuthorizationHandler : AuthorizationHandler<BonPermissionRequirement>
{
    private readonly IBonPermissionManager _permissionManager;
    private readonly IBonCurrentUser _bonCurrentUser;
    private readonly ILogger<BonPermissionAuthorizationHandler> _logger;

    public BonPermissionAuthorizationHandler(
        IBonPermissionManager permissionManager,
        ILogger<BonPermissionAuthorizationHandler> logger)
    {
        _permissionManager = permissionManager;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BonPermissionRequirement requirement)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            _logger.LogDebug("User is not authenticated, denying permission '{Permission}'", requirement.Permission);
            return;
        }

        try
        {
            var userId = _bonCurrentUser.Id;
            
            if (!userId.HasValue)
            {
                _logger.LogWarning("User ID not found in claims, denying permission '{Permission}'", requirement.Permission);
                return;
            }

            var requiredPermissions = requirement.GetAllPermissions().ToList();
            var hasPermissions = new List<bool>();

            // Check each required permission
            foreach (var permission in requiredPermissions)
            {
                var hasPermission = await CheckPermissionAsync(context.User, BonUserId.NewId(userId.Value), permission);
                hasPermissions.Add(hasPermission);
                
                _logger.LogDebug("Permission check for '{Permission}': {Result} (User: {UserId})", 
                    permission, hasPermission ? "GRANTED" : "DENIED", userId);
            }

            // Apply logic (AND/OR)
            bool authorized;
            if (requirement.RequireAll)
            {
                // AND logic - all permissions must be granted
                authorized = hasPermissions.All(h => h);
            }
            else
            {
                // OR logic - at least one permission must be granted
                authorized = hasPermissions.Any(h => h);
            }

            if (authorized)
            {
                context.Succeed(requirement);
                _logger.LogInformation("Authorization succeeded for user '{UserId}' with requirement: {Requirement}", 
                    userId, requirement);
            }
            else
            {
                _logger.LogWarning("Authorization failed for user '{UserId}' with requirement: {Requirement}", 
                    userId, requirement);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authorization for requirement: {Requirement}", requirement);
        }
    }

    /// <summary>
    /// Checks if a user has a specific permission through direct claims or role claims
    /// </summary>
    private async Task<bool> CheckPermissionAsync(ClaimsPrincipal user, BonUserId userId, string permission)
    {
        try
        {
            // First check direct user claims
            if (HasDirectPermissionClaim(user, permission))
            {
                _logger.LogDebug("User '{UserId}' has direct permission claim for '{Permission}'", userId, permission);
                return true;
            }

            // Then check role-based permissions
            if (await HasRoleBasedPermissionAsync(userId, permission))
            {
                _logger.LogDebug("User '{UserId}' has role-based permission for '{Permission}'", userId, permission);
                return true;
            }

            // Check hierarchical permissions (if user has parent permission)
            if (await HasHierarchicalPermissionAsync(userId, permission))
            {
                _logger.LogDebug("User '{UserId}' has hierarchical permission for '{Permission}'", userId, permission);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission '{Permission}' for user '{UserId}'", permission, userId);
            return false;
        }
    }

    /// <summary>
    /// Checks if user has direct permission claim
    /// </summary>
    private static bool HasDirectPermissionClaim(ClaimsPrincipal user, string permission)
    {
        return user.HasClaim(BonClaimTypes.Permission, permission);
    }

    /// <summary>
    /// Checks if user has permission through roles
    /// </summary>
    private async Task<bool> HasRoleBasedPermissionAsync(BonUserId userId, string permission)
    {
        try
        {
            var userPermissions = await _permissionManager.GetUserPermissionsAsync(userId);
            return userPermissions.Contains(permission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role-based permissions for user '{UserId}'", userId);
            return false;
        }
    }

    /// <summary>
    /// Checks if user has permission through hierarchical inheritance
    /// </summary>
    private async Task<bool> HasHierarchicalPermissionAsync(BonUserId userId, string permission)
    {
        try
        {
            var permissionDef = _permissionManager.GetPermission(permission);
            if (permissionDef?.Parent == null)
            {
                return false;
            }

            // Check if user has parent permission
            var userPermissions = await _permissionManager.GetUserPermissionsAsync(userId);
            
            // Traverse up the hierarchy
            var currentPermission = permissionDef.Parent;
            while (currentPermission != null)
            {
                if (userPermissions.Contains(currentPermission.Name))
                {
                    _logger.LogDebug("User '{UserId}' has parent permission '{ParentPermission}' for '{Permission}'", 
                        userId, currentPermission.Name, permission);
                    return true;
                }
                currentPermission = currentPermission.Parent;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking hierarchical permissions for user '{UserId}' and permission '{Permission}'", 
                userId, permission);
            return false;
        }
    }

    /// <summary>
    /// Extracts user ID from claims
    /// </summary>
    private static string? GetUserId(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
               user.FindFirst("sub")?.Value ??
               user.FindFirst("id")?.Value ??
               user.FindFirst("user_id")?.Value;
    }
} 