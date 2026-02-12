namespace Bonyan.IdentityManagement.ClaimProvider;

/// <summary>
/// Context passed to claim providers. Contains only primitive/simple data so that other modules
/// can implement <see cref="IBonIdentityClaimProvider"/> without referencing the Identity domain.
/// Aligns with clean architecture: providers depend on this contract, not on domain entities.
/// </summary>
public class BonIdentityClaimProviderContext
{
    /// <summary>User identifier (string representation).</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>User name (login name).</summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>Email address if set.</summary>
    public string? Email { get; set; }

    /// <summary>Whether the email is verified.</summary>
    public bool EmailVerified { get; set; }

    /// <summary>Phone number if set.</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>Whether the phone is verified.</summary>
    public bool PhoneVerified { get; set; }

    /// <summary>First name from profile.</summary>
    public string? FirstName { get; set; }

    /// <summary>Last name from profile.</summary>
    public string? LastName { get; set; }

    /// <summary>User status name (e.g. Active, Locked).</summary>
    public string? Status { get; set; }

    /// <summary>UTC creation date (ISO string or empty).</summary>
    public string CreatedAt { get; set; } = string.Empty;

    /// <summary>Role names the user has (for role-based claims).</summary>
    public IReadOnlyList<string> RoleNames { get; set; } = Array.Empty<string>();

    /// <summary>Permission names the user has (claim type bon.permission).</summary>
    public IReadOnlyList<string> PermissionNames { get; set; } = Array.Empty<string>();

    /// <summary>Optional tenant identifier (multi-tenant apps).</summary>
    public string? TenantId { get; set; }

    /// <summary>Extra key-value data for custom claim providers.</summary>
    public IReadOnlyDictionary<string, string> AdditionalData { get; set; } = new Dictionary<string, string>();
}
