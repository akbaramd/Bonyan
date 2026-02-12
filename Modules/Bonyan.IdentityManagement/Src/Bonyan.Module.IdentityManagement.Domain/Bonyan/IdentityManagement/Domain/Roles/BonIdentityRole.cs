using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Aggregate;

namespace Bonyan.IdentityManagement.Domain.Roles;

/// <summary>
/// Role aggregate root. Contains entities: RoleClaims, RoleMeta.
/// </summary>
public class BonIdentityRole : BonAggregateRoot<BonRoleId>
{
    private readonly List<BonIdentityRoleClaims> _claims = new();
    private readonly List<BonIdentityRoleMeta> _metas = new();

    public string Title { get; private set; } = string.Empty;
    public bool CanBeDeleted { get; private set; } = true;
    public IReadOnlyCollection<BonIdentityRoleClaims> RoleClaims => _claims;
    public IReadOnlyCollection<BonIdentityRoleMeta> RoleMetas => _metas;

    protected BonIdentityRole() { }

    public BonIdentityRole(BonRoleId id, string title, bool canBeDeleted = true)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        SetTitle(title);
        CanBeDeleted = canBeDeleted;
    }

    public void UpdateTitle(string newTitle) => SetTitle(newTitle);

    public void AddClaim(BonRoleClaimId claimId, string claimType, string claimValue, string? issuer = null)
    {
        if (claimId == null) throw new ArgumentNullException(nameof(claimId));
        if (string.IsNullOrEmpty(claimType)) throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));
        if (string.IsNullOrEmpty(claimValue)) throw new ArgumentException("Claim value cannot be null or empty.", nameof(claimValue));

        if (_claims.All(c => c.ClaimType != claimType || c.ClaimValue != claimValue))
            _claims.Add(new BonIdentityRoleClaims(claimId, Id, claimType, claimValue, issuer));
    }

    public void RemoveClaim(string claimType, string claimValue)
    {
        if (string.IsNullOrEmpty(claimType)) throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));
        if (string.IsNullOrEmpty(claimValue)) throw new ArgumentException("Claim value cannot be null or empty.", nameof(claimValue));

        var roleClaim = _claims.FirstOrDefault(c => c.ClaimType == claimType && c.ClaimValue == claimValue);
        if (roleClaim != null) _claims.Remove(roleClaim);
    }

    public void RemoveClaimsByType(string claimType)
    {
        if (string.IsNullOrEmpty(claimType)) throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));
        foreach (var claim in _claims.Where(c => c.ClaimType == claimType).ToList())
            _claims.Remove(claim);
    }

    public bool HasClaim(string claimType, string claimValue)
    {
        if (string.IsNullOrEmpty(claimType)) throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));
        if (string.IsNullOrEmpty(claimValue)) throw new ArgumentException("Claim value cannot be null or empty.", nameof(claimValue));
        return _claims.Any(c => c.ClaimType == claimType && c.ClaimValue == claimValue);
    }

    public IEnumerable<BonIdentityRoleClaims> GetClaimsByType(string claimType)
    {
        if (string.IsNullOrEmpty(claimType)) throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));
        return _claims.Where(c => c.ClaimType == claimType);
    }

    public void SetMeta(string metaKey, string metaValue)
    {
        if (string.IsNullOrWhiteSpace(metaKey)) throw new ArgumentException("Meta key cannot be null or empty.", nameof(metaKey));
        var meta = _metas.FirstOrDefault(m => m.MetaKey == metaKey);
        if (meta != null)
            meta.UpdateValue(metaValue);
        else
            _metas.Add(new BonIdentityRoleMeta(Id, metaKey, metaValue));
    }

    public void RemoveMeta(string metaKey)
    {
        if (string.IsNullOrWhiteSpace(metaKey)) throw new ArgumentException("Meta key cannot be null or empty.", nameof(metaKey));
        var meta = _metas.FirstOrDefault(m => m.MetaKey == metaKey);
        if (meta != null) _metas.Remove(meta);
    }

    public string? GetMetaValue(string metaKey) =>
        _metas.FirstOrDefault(m => m.MetaKey == metaKey)?.MetaValue;

    public void MarkAsNonDeletable() => CanBeDeleted = false;
    public void MarkAsDeletable() => CanBeDeleted = true;

    public void Delete()
    {
        if (!CanBeDeleted)
            throw new InvalidOperationException("This role cannot be deleted because it is marked as non-deletable.");
    }

    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));
        Title = title;
    }
}
