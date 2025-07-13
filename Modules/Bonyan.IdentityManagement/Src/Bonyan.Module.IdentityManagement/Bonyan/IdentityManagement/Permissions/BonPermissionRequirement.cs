using Microsoft.AspNetCore.Authorization;

namespace Bonyan.IdentityManagement.Permissions;

/// <summary>
/// Represents a permission requirement for authorization
/// </summary>
public class BonPermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The required permission name
    /// </summary>
    public string Permission { get; }

    /// <summary>
    /// Whether to require all permissions (for multiple permissions)
    /// </summary>
    public bool RequireAll { get; }

    /// <summary>
    /// Additional permissions for complex requirements
    /// </summary>
    public string[] AdditionalPermissions { get; }

    public BonPermissionRequirement(string permission)
    {
        Permission = permission ?? throw new ArgumentNullException(nameof(permission));
        RequireAll = false;
        AdditionalPermissions = Array.Empty<string>();
    }

    public BonPermissionRequirement(string permission, bool requireAll, params string[] additionalPermissions)
    {
        Permission = permission ?? throw new ArgumentNullException(nameof(permission));
        RequireAll = requireAll;
        AdditionalPermissions = additionalPermissions ?? Array.Empty<string>();
    }

    /// <summary>
    /// Gets all permissions required by this requirement
    /// </summary>
    public IEnumerable<string> GetAllPermissions()
    {
        yield return Permission;
        
        foreach (var additional in AdditionalPermissions)
        {
            yield return additional;
        }
    }

    public override string ToString()
    {
        if (AdditionalPermissions.Any())
        {
            var allPermissions = string.Join(", ", GetAllPermissions());
            var logic = RequireAll ? "AND" : "OR";
            return $"Permission: {allPermissions} (Logic: {logic})";
        }
        
        return $"Permission: {Permission}";
    }
} 