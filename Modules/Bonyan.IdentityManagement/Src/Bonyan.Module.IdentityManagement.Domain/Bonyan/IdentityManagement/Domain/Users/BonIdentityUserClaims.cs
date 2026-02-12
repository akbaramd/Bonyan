using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.Layer.Domain.Entities;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users;

/// <summary>
/// ادعای کاربر — نوع نهایی بدون جنریک.
/// </summary>
public class BonIdentityUserClaims : BonEntity<BonUserClaimId>
{
    public BonUserId UserId { get; private set; } = null!;
    public string ClaimType { get; private set; } = string.Empty;
    public string ClaimValue { get; private set; } = string.Empty;
    public string? Issuer { get; private set; }
    public string? ClaimValueType { get; set; }

    protected BonIdentityUserClaims() { }

    public BonIdentityUserClaims(BonUserClaimId id, BonUserId userId, string claimType, string claimValue, string? claimValueType, string? issuer = null)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        ClaimType = claimType ?? throw new ArgumentNullException(nameof(claimType));
        ClaimValue = claimValue ?? throw new ArgumentNullException(nameof(claimValue));
        ClaimValueType = claimValueType;
        Issuer = issuer;
    }

    public void UpdateClaimValue(string newValue) => ClaimValue = newValue ?? throw new ArgumentNullException(nameof(newValue));
    public void UpdateClaimType(string newType) => ClaimType = newType ?? throw new ArgumentNullException(nameof(newType));
    public void UpdateIssuer(string? newIssuer) => Issuer = newIssuer;
}
