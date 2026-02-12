namespace Bonyan.IdentityManagement.Domain;

/// <summary>
/// Claim type constants for identity and permissions (ubiquitous language of the domain).
/// Kept in Domain so that domain entities and services can reference them without depending on host modules.
/// Application and other modules can use these same constants when adding permissions/claims to users.
/// </summary>
public static class BonIdentityClaimTypes
{
    /// <summary>
    /// Claim type for permissions. To grant a permission to a user, add a user claim with this type and value = permission name.
    /// </summary>
    public const string Permission = "bon.permission";

    /// <summary>
    /// Claim type for features.
    /// </summary>
    public const string Feature = "bon.feature";

    /// <summary>
    /// Claim type for module access.
    /// </summary>
    public const string ModuleAccess = "bon.module_access";
}
