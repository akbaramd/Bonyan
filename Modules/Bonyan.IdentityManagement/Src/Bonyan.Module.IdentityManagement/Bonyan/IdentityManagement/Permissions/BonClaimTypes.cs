namespace Bonyan.IdentityManagement.Permissions;

/// <summary>
/// Defines the claim types used in the Bonyan identity system
/// </summary>
public static class BonClaimTypes
{
    /// <summary>
    /// Claim type for permissions
    /// </summary>
    public const string Permission = "bon.permission";
    
    /// <summary>
    /// Claim type for features
    /// </summary>
    public const string Feature = "bon.feature";
    
    /// <summary>
    /// Claim type for module access
    /// </summary>
    public const string ModuleAccess = "bon.module_access";
} 