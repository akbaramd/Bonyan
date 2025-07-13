using System.Collections.Concurrent;
using Bonyan.IdentityManagement.Domain.Roles.DomainServices;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.Permissions;

/// <summary>
/// Implementation of permission manager
/// </summary>
public class BonPermissionManager : IBonPermissionManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BonPermissionManager> _logger;
    private readonly ConcurrentDictionary<string, PermissionDefinition> _permissions = new();
    private readonly ConcurrentDictionary<string, PermissionGroupDefinition> _groups = new();
    private readonly object _initializationLock = new();
    private bool _isInitialized = false;

    public BonPermissionManager(
        IServiceProvider serviceProvider,
        ILogger<BonPermissionManager> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void InitializePermissions()
    {
        if (_isInitialized)
            return;

        lock (_initializationLock)
        {
            if (_isInitialized)
                return;

            try
            {
                _logger.LogInformation("Initializing permissions from providers...");

                var providers = _serviceProvider.GetServices<IBonPermissionDefinitionProvider>();
                var context = new PermissionDefinitionContext();

                foreach (var provider in providers)
                {
                    try
                    {
                        provider.Define(context);
                        _logger.LogDebug("Permissions defined by provider: {ProviderType}", provider.GetType().Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error defining permissions from provider: {ProviderType}", provider.GetType().Name);
                    }
                }

                // Store groups
                foreach (var group in context.Groups)
                {
                    _groups[group.Name] = group;
                }

                // Store all permissions (including nested ones)
                foreach (var permission in context.GetAllPermissions())
                {
                    _permissions[permission.Name] = permission;
                }

                _isInitialized = true;
                _logger.LogInformation("Permission initialization completed. Loaded {PermissionCount} permissions in {GroupCount} groups",
                    _permissions.Count, _groups.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize permissions");
                throw;
            }
        }
    }

    public IEnumerable<PermissionDefinition> GetAllPermissions()
    {
        EnsureInitialized();
        return _permissions.Values;
    }

    public IEnumerable<PermissionGroupDefinition> GetAllGroups()
    {
        EnsureInitialized();
        return _groups.Values;
    }

    public PermissionDefinition? GetPermission(string name)
    {
        EnsureInitialized();
        return _permissions.TryGetValue(name, out var permission) ? permission : null;
    }

    public PermissionGroupDefinition? GetGroup(string name)
    {
        EnsureInitialized();
        return _groups.TryGetValue(name, out var group) ? group : null;
    }

    public bool PermissionExists(string name)
    {
        EnsureInitialized();
        return _permissions.ContainsKey(name);
    }

    public IEnumerable<PermissionDefinition> GetPermissionsForGroup(string groupName)
    {
        EnsureInitialized();
        var group = GetGroup(groupName);
        return group?.GetAllPermissions() ?? Enumerable.Empty<PermissionDefinition>();
    }

    public IEnumerable<PermissionDefinition> GetChildPermissions(string parentPermissionName)
    {
        EnsureInitialized();
        var parent = GetPermission(parentPermissionName);
        return parent?.Children ?? Enumerable.Empty<PermissionDefinition>();
    }

    public async Task GrantPermissionToUserAsync(BonUserId userId, string permission, string? issuer = null)
    {
        EnsureInitialized();
        
        if (!PermissionExists(permission))
        {
            throw new ArgumentException($"Permission '{permission}' does not exist", nameof(permission));
        }

        try
        {
            var userManager = _serviceProvider.GetRequiredService<IBonIdentityUserManager<BonIdentityUser>>();
            var userResult = await userManager.FindByIdAsync(userId);
            
            if (!userResult.IsSuccess)
            {
                throw new InvalidOperationException($"User with ID '{userId}' not found");
            }

            var user = userResult.Value;
            await userManager.AddClaimAsync(user, BonClaimTypes.Permission, permission, issuer);
            
            _logger.LogInformation("Granted permission '{Permission}' to user '{UserId}'", permission, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to grant permission '{Permission}' to user '{UserId}'", permission, userId);
            throw;
        }
    }

    public async Task RevokePermissionFromUserAsync(BonUserId userId, string permission)
    {
        try
        {
            var userManager = _serviceProvider.GetRequiredService<IBonIdentityUserManager<BonIdentityUser>>();
            var userResult = await userManager.FindByIdAsync(userId);
            
            if (!userResult.IsSuccess)
            {
                throw new InvalidOperationException($"User with ID '{userId}' not found");
            }

            var user = userResult.Value;
            await userManager.RemoveClaimAsync(user, BonClaimTypes.Permission, permission);
            
            _logger.LogInformation("Revoked permission '{Permission}' from user '{UserId}'", permission, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to revoke permission '{Permission}' from user '{UserId}'", permission, userId);
            throw;
        }
    }

    public async Task GrantPermissionToRoleAsync(BonRoleId roleId, string permission, string? issuer = null)
    {
        EnsureInitialized();
        
        if (!PermissionExists(permission))
        {
            throw new ArgumentException($"Permission '{permission}' does not exist", nameof(permission));
        }

        try
        {
            var roleManager = _serviceProvider.GetRequiredService<IBonIdentityRoleManager>();
            var roleResult = await roleManager.FindRoleByIdAsync(roleId.Value);
            
            if (!roleResult.IsSuccess)
            {
                throw new InvalidOperationException($"Role with ID '{roleId}' not found");
            }

            var role = roleResult.Value;
            await roleManager.AddClaimToRoleAsync(role, BonClaimTypes.Permission, permission, issuer);
            
            _logger.LogInformation("Granted permission '{Permission}' to role '{RoleId}'", permission, roleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to grant permission '{Permission}' to role '{RoleId}'", permission, roleId);
            throw;
        }
    }

    public async Task RevokePermissionFromRoleAsync(BonRoleId roleId, string permission)
    {
        try
        {
            var roleManager = _serviceProvider.GetRequiredService<IBonIdentityRoleManager>();
            var roleResult = await roleManager.FindRoleByIdAsync(roleId.Value);
            
            if (!roleResult.IsSuccess)
            {
                throw new InvalidOperationException($"Role with ID '{roleId}' not found");
            }

            var role = roleResult.Value;
            await roleManager.RemoveClaimFromRoleAsync(role, BonClaimTypes.Permission, permission);
            
            _logger.LogInformation("Revoked permission '{Permission}' from role '{RoleId}'", permission, roleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to revoke permission '{Permission}' from role '{RoleId}'", permission, roleId);
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(BonUserId userId)
    {
        try
        {
            var userManager = _serviceProvider.GetRequiredService<IBonIdentityUserManager<BonIdentityUser>>();
            var userResult = await userManager.FindByIdAsync(userId);
            
            if (!userResult.IsSuccess)
            {
                return Enumerable.Empty<string>();
            }

            var user = userResult.Value;
            var permissions = new HashSet<string>();

            // Get direct user permissions
            var userClaimsResult = await userManager.GetClaimsByTypeAsync(user, BonClaimTypes.Permission);
            if (userClaimsResult.IsSuccess)
            {
                foreach (var claim in userClaimsResult.Value)
                {
                    permissions.Add(claim.ClaimValue);
                }
            }

            // Get permissions from user roles
            var userRolesResult = await userManager.GetUserRolesAsync(user);
            if (userRolesResult.IsSuccess)
            {
                var roleManager = _serviceProvider.GetRequiredService<IBonIdentityRoleManager>();
                
                foreach (var role in userRolesResult.Value)
                {
                    var roleClaimsResult = await roleManager.GetClaimsByTypeAsync(role, BonClaimTypes.Permission);
                    if (roleClaimsResult.IsSuccess)
                    {
                        foreach (var claim in roleClaimsResult.Value)
                        {
                            permissions.Add(claim.ClaimValue);
                        }
                    }
                }
            }

            return permissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get permissions for user '{UserId}'", userId);
            return Enumerable.Empty<string>();
        }
    }

    public async Task<IEnumerable<string>> GetRolePermissionsAsync(BonRoleId roleId)
    {
        try
        {
            var roleManager = _serviceProvider.GetRequiredService<IBonIdentityRoleManager>();
            var roleResult = await roleManager.FindRoleByIdAsync(roleId.Value);
            
            if (!roleResult.IsSuccess)
            {
                return Enumerable.Empty<string>();
            }

            var role = roleResult.Value;
            var claimsResult = await roleManager.GetClaimsByTypeAsync(role, BonClaimTypes.Permission);
            
            if (claimsResult.IsSuccess)
            {
                return claimsResult.Value.Select(c => c.ClaimValue);
            }

            return Enumerable.Empty<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get permissions for role '{RoleId}'", roleId);
            return Enumerable.Empty<string>();
        }
    }

    public async Task<bool> HasPermissionAsync(BonUserId userId, string permission)
    {
        var userPermissions = await GetUserPermissionsAsync(userId);
        return userPermissions.Contains(permission);
    }

    private void EnsureInitialized()
    {
        if (!_isInitialized)
        {
            InitializePermissions();
        }
    }
} 