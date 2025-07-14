using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Entities;

namespace Bonyan.IdentityManagement.Domain.Roles;

/// <summary>
/// Represents a claim associated with a role in the identity system.
/// </summary>
public class BonIdentityRoleClaims<TRole> : BonEntity<BonRoleClaimId>
    where TRole : BonIdentityRole<TRole>
{
    /// <summary>
    /// Gets the ID of the role associated with this claim.
    /// </summary>
    public BonRoleId RoleId { get; private set; }

    /// <summary>
    /// Gets the type of the claim (e.g., "permission", "feature").
    /// </summary>
    public string ClaimType { get; private set; }

    /// <summary>
    /// Gets the value of the claim.
    /// </summary>
    public string ClaimValue { get; private set; }

    /// <summary>
    /// Gets the issuer of the claim (optional).
    /// </summary>
    public string? Issuer { get; private set; }

    // Navigation property back to role
    public TRole Role { get; private set; } = default!;

    // Protected constructor for EF Core
    protected BonIdentityRoleClaims() { }

    /// <summary>
    /// Constructs a new role claim.
    /// </summary>
    public BonIdentityRoleClaims(BonRoleClaimId id, BonRoleId roleId, string claimType, string claimValue, string? issuer = null)
    {
        Id = id;
        RoleId = roleId ?? throw new ArgumentNullException(nameof(roleId));
        ClaimType = claimType ?? throw new ArgumentNullException(nameof(claimType));
        ClaimValue = claimValue ?? throw new ArgumentNullException(nameof(claimValue));
        Issuer = issuer;
    }

    /// <summary>
    /// Updates the claim value.
    /// </summary>
    public void UpdateClaimValue(string newValue)
    {
        ClaimValue = newValue ?? throw new ArgumentNullException(nameof(newValue));
    }

    /// <summary>
    /// Updates the claim type.
    /// </summary>
    public void UpdateClaimType(string newType)
    {
        ClaimType = newType ?? throw new ArgumentNullException(nameof(newType));
    }

    /// <summary>
    /// Updates the issuer.
    /// </summary>
    public void UpdateIssuer(string? newIssuer)
    {
        Issuer = newIssuer;
    }
} 