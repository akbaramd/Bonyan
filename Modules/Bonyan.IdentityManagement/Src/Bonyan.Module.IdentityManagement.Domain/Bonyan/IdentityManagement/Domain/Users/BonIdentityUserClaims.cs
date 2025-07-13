using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.Layer.Domain.Entities;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users;

/// <summary>
/// Represents a claim associated with a user in the identity system.
/// </summary>
public class BonIdentityUserClaims : BonEntity<BonUserClaimId>
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

    // Navigation property back to user
    public BonIdentityUser User { get; private set; } = default!;

    // Protected constructor for EF Core
    protected BonIdentityUserClaims() { }

    /// <summary>
    /// Constructs a new user claim.
    /// </summary>
    public BonIdentityUserClaims(BonUserClaimId id, BonUserId userId, string claimType, string claimValue, string? issuer = null) 
        
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
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