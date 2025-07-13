using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Bonyan.IdentityManagement.Permissions;

/// <summary>
/// Dynamic policy provider that creates authorization policies for permissions
/// </summary>
public class BonPermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly IBonPermissionManager _permissionManager;
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public BonPermissionPolicyProvider(
        IBonPermissionManager permissionManager,
        IOptions<AuthorizationOptions> options)
    {
        _permissionManager = permissionManager;
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Check if this is a permission-based policy
        if (IsPermissionPolicy(policyName))
        {
            return CreatePermissionPolicy(policyName);
        }

        // Fall back to default policy provider for non-permission policies
        return await _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }

    public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return await _fallbackPolicyProvider.GetDefaultPolicyAsync();
    }

    public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return await _fallbackPolicyProvider.GetFallbackPolicyAsync();
    }

    /// <summary>
    /// Determines if a policy name represents a permission
    /// </summary>
    private bool IsPermissionPolicy(string policyName)
    {
        if (string.IsNullOrWhiteSpace(policyName))
            return false;

        // Check if the policy name is a valid permission
        return _permissionManager.PermissionExists(policyName);
    }

    /// <summary>
    /// Creates an authorization policy for a permission
    /// </summary>
    private AuthorizationPolicy CreatePermissionPolicy(string permissionName)
    {
        var policyBuilder = new AuthorizationPolicyBuilder();
        
        // Add authentication requirement
        policyBuilder.RequireAuthenticatedUser();
        
        // Add permission requirement
        policyBuilder.AddRequirements(new BonPermissionRequirement(permissionName));
        
        return policyBuilder.Build();
    }
}

/// <summary>
/// Extensions for permission-based authorization
/// </summary>
public static class PermissionPolicyExtensions
{
    /// <summary>
    /// Adds a permission requirement to the policy
    /// </summary>
    public static AuthorizationPolicyBuilder RequirePermission(
        this AuthorizationPolicyBuilder builder,
        string permission)
    {
        return builder.AddRequirements(new BonPermissionRequirement(permission));
    }

    /// <summary>
    /// Adds multiple permission requirements with AND logic
    /// </summary>
    public static AuthorizationPolicyBuilder RequireAllPermissions(
        this AuthorizationPolicyBuilder builder,
        params string[] permissions)
    {
        if (permissions == null || permissions.Length == 0)
            throw new ArgumentException("At least one permission is required", nameof(permissions));

        if (permissions.Length == 1)
        {
            return builder.RequirePermission(permissions[0]);
        }

        var firstPermission = permissions[0];
        var additionalPermissions = permissions.Skip(1).ToArray();
        
        return builder.AddRequirements(
            new BonPermissionRequirement(firstPermission, true, additionalPermissions));
    }

    /// <summary>
    /// Adds multiple permission requirements with OR logic
    /// </summary>
    public static AuthorizationPolicyBuilder RequireAnyPermission(
        this AuthorizationPolicyBuilder builder,
        params string[] permissions)
    {
        if (permissions == null || permissions.Length == 0)
            throw new ArgumentException("At least one permission is required", nameof(permissions));

        if (permissions.Length == 1)
        {
            return builder.RequirePermission(permissions[0]);
        }

        var firstPermission = permissions[0];
        var additionalPermissions = permissions.Skip(1).ToArray();
        
        return builder.AddRequirements(
            new BonPermissionRequirement(firstPermission, false, additionalPermissions));
    }
} 