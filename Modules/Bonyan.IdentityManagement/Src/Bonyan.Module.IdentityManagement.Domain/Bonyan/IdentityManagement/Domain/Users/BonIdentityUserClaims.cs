using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.Layer.Domain.Entities;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users;

/// <summary>
/// Represents a claim associated with a user in the identity system.
/// </summary>
public class BonIdentityUserClaims<TUser,TRole> : BonEntity<BonUserClaimId>
    where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
    /// <summary>
    /// Gets the ID of the user associated with this claim.
    /// </summary>
    public BonUserId UserId { get; private set; }

    /// <summary>
    /// Gets the type of the claim (e.g., "role", "permission", "email").
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

    public string? ClaimValueType { get; set; }

    // Protected constructor for EF Core
    protected BonIdentityUserClaims() { }

    /// <summary>
    /// Constructs a new user claim.
    /// </summary>
    public BonIdentityUserClaims(BonUserClaimId id, BonUserId userId, string claimType, string claimValue, string? claimValueType, string? issuer = null)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        ClaimType = claimType ?? throw new ArgumentNullException(nameof(claimType));
        ClaimValue = claimValue ?? throw new ArgumentNullException(nameof(claimValue));
        ClaimValueType = claimValueType;
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