using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Entities;

namespace Bonyan.IdentityManagement.Domain.Roles;

/// <summary>
/// ادعای مرتبط با نقش — بدون جنریک.
/// </summary>
public class BonIdentityRoleClaims : BonEntity<BonRoleClaimId>
{
    public BonRoleId RoleId { get; private set; } = null!;
    public string ClaimType { get; private set; } = string.Empty;
    public string ClaimValue { get; private set; } = string.Empty;
    public string? Issuer { get; private set; }

    public BonIdentityRole Role { get; private set; } = null!;

    protected BonIdentityRoleClaims() { }

    public BonIdentityRoleClaims(BonRoleClaimId id, BonRoleId roleId, string claimType, string claimValue, string? issuer = null)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        RoleId = roleId ?? throw new ArgumentNullException(nameof(roleId));
        ClaimType = claimType ?? throw new ArgumentNullException(nameof(claimType));
        ClaimValue = claimValue ?? throw new ArgumentNullException(nameof(claimValue));
        Issuer = issuer;
    }

    public void UpdateClaimValue(string newValue) => ClaimValue = newValue ?? throw new ArgumentNullException(nameof(newValue));
    public void UpdateClaimType(string newType) => ClaimType = newType ?? throw new ArgumentNullException(nameof(newType));
    public void UpdateIssuer(string? newIssuer) => Issuer = newIssuer;
}
