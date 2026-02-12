using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users.DomainEvents;
using Bonyan.IdentityManagement.Domain.Users.UserMeta;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.IdentityManagement.Domain;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users;

/// <summary>
/// User aggregate root. Extends BonUser with password, tokens, roles, claims and meta.
/// Entities in this aggregate: Tokens (hashed), UserRoles, UserClaims, UserMeta.
/// UserTokens = security tokens (refresh, etc.); UserMeta = extensible key-value metadata (different usage).
/// </summary>
public class BonIdentityUser : BonUser
{
    private readonly List<BonIdentityUserToken> _tokens = new();
    private readonly List<BonIdentityUserRoles> _roles = new();
    private readonly List<BonIdentityUserClaims> _claims = new();
    private readonly List<BonUserMeta> _metas = new();

    public BonUserPassword Password { get; private set; } = null!;
    public bool CanBeDeleted { get; private set; } = true;
    public DateTime? BannedUntil { get; private set; }
    public int FailedLoginAttemptCount { get; private set; }
    public DateTime? AccountLockedUntil { get; private set; }

    public IReadOnlyCollection<BonIdentityUserToken> Tokens => _tokens;
    public IReadOnlyCollection<BonIdentityUserRoles> UserRoles => _roles;
    public IReadOnlyCollection<BonIdentityUserClaims> UserClaims => _claims;
    public IReadOnlyCollection<BonUserMeta> Metas => _metas;

    protected BonIdentityUser() { }

    public BonIdentityUser(BonUserId id, string userName, UserProfile profile)
        : base(id, userName, profile)
    {
    }

    public BonIdentityUser(BonUserId id, string userName, string password, UserProfile profile)
        : base(id, userName, profile)
    {
        SetPassword(password);
    }

    public BonIdentityUser(BonUserId id, string userName, string password, BonUserEmail email, BonUserPhoneNumber phoneNumber, UserProfile profile)
        : base(id, userName, profile, email, phoneNumber)
    {
        SetPassword(password);
    }

    public void SetPassword(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Password cannot be empty.", nameof(newPassword));
        Password = new BonUserPassword(newPassword);
        AddDomainEvent(new BonIdentityUserPasswordChangedDomainEvent(this));
    }

    public bool VerifyPassword(string password) => Password.Verify(password);

    /// <summary>
    /// Sets the token for this user. Caller must pass the already-hashed value so it is never stored in plain form.
    /// </summary>
    public void SetToken(string tokenType, string hashedValue, DateTime? expiration = null)
    {
        if (string.IsNullOrWhiteSpace(tokenType)) throw new ArgumentException("Token type cannot be empty.", nameof(tokenType));
        if (string.IsNullOrWhiteSpace(hashedValue)) throw new ArgumentException("Token hashed value cannot be empty.", nameof(hashedValue));

        var token = _tokens.FirstOrDefault(t => t.Type == tokenType);
        if (token != null)
        {
            token.UpdateValue(hashedValue, expiration);
            AddDomainEvent(new BonIdentityUserTokenUpdatedDomainEvent(this, token));
        }
        else
        {
            token = new BonIdentityUserToken(Id, tokenType, hashedValue, expiration);
            _tokens.Add(token);
            AddDomainEvent(new BonIdentityUserTokenAddedDomainEvent(this, token));
        }
    }

    public void RemoveToken(string tokenType)
    {
        var token = _tokens.FirstOrDefault(t => t.Type == tokenType);
        if (token == null) throw new InvalidOperationException($"No token of type {tokenType} exists for this user.");
        _tokens.Remove(token);
        AddDomainEvent(new BonIdentityUserTokenRemovedDomainEvent(this, token));
    }

    public bool IsTokenExpired(string tokenType)
    {
        var token = _tokens.FirstOrDefault(t => t.Type == tokenType);
        if (token == null) throw new InvalidOperationException($"No token of type {tokenType} exists for this user.");
        return token.IsExpired();
    }

    public void AssignRole(BonRoleId roleId)
    {
        if (_roles.Any(r => r.RoleId == roleId))
            throw new InvalidOperationException($"User already has the role with ID {roleId.Value}.");
        _roles.Add(new BonIdentityUserRoles(Id, roleId));
        AddDomainEvent(new BonIdentityUserRoleAddedDomainEvent(Id, roleId));
    }

    public void RemoveRole(BonRoleId roleId)
    {
        var role = _roles.FirstOrDefault(r => r.RoleId == roleId);
        if (role == null) throw new InvalidOperationException($"User does not have the role with ID {roleId.Value}.");
        _roles.Remove(role);
        AddDomainEvent(new BonIdentityUserRoleRemovedDomainEvent(Id, roleId));
    }

    public void AddClaim(string claimType, string claimValue, string? issuer = null)
    {
        if (string.IsNullOrEmpty(claimType)) throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));
        if (string.IsNullOrEmpty(claimValue)) throw new ArgumentException("Claim value cannot be null or empty.", nameof(claimValue));
        if (_claims.All(c => c.ClaimType != claimType || c.ClaimValue != claimValue))
            _claims.Add(new BonIdentityUserClaims(BonUserClaimId.NewId(), Id, claimType, claimValue, null, issuer));
    }

    public void AddClaim(BonUserClaimId claimId, string claimType, string claimValue, string? issuer = null)
    {
        if (claimId == null) throw new ArgumentNullException(nameof(claimId));
        if (string.IsNullOrEmpty(claimType)) throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));
        if (string.IsNullOrEmpty(claimValue)) throw new ArgumentException("Claim value cannot be null or empty.", nameof(claimValue));
        if (_claims.All(c => c.ClaimType != claimType || c.ClaimValue != claimValue))
            _claims.Add(new BonIdentityUserClaims(claimId, Id, claimType, claimValue, null, issuer));
    }

    public void RemoveClaim(string claimType, string claimValue)
    {
        if (string.IsNullOrEmpty(claimType)) throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));
        if (string.IsNullOrEmpty(claimValue)) throw new ArgumentException("Claim value cannot be null or empty.", nameof(claimValue));
        var userClaim = _claims.FirstOrDefault(c => c.ClaimType == claimType && c.ClaimValue == claimValue);
        if (userClaim != null) _claims.Remove(userClaim);
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

    public IEnumerable<BonIdentityUserClaims> GetClaimsByType(string claimType)
    {
        if (string.IsNullOrEmpty(claimType)) throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));
        return _claims.Where(c => c.ClaimType == claimType);
    }

    /// <summary>Adds a permission to the user (stored as a claim with type <see cref="BonIdentityClaimTypes.Permission"/>).</summary>
    public void AddPermission(string permissionName, string? issuer = null) =>
        AddClaim(BonIdentityClaimTypes.Permission, permissionName ?? throw new ArgumentNullException(nameof(permissionName)), issuer);

    /// <summary>Removes a permission claim from the user.</summary>
    public void RemovePermission(string permissionName) =>
        RemoveClaim(BonIdentityClaimTypes.Permission, permissionName ?? throw new ArgumentNullException(nameof(permissionName)));

    /// <summary>Returns true if the user has the given permission (claim type Permission).</summary>
    public bool HasPermission(string permissionName) =>
        HasClaim(BonIdentityClaimTypes.Permission, permissionName ?? throw new ArgumentNullException(nameof(permissionName)));

    public void SetMeta(string metaKey, string metaValue)
    {
        if (string.IsNullOrWhiteSpace(metaKey)) throw new ArgumentException("Meta key cannot be null or empty.", nameof(metaKey));
        var meta = _metas.FirstOrDefault(m => m.MetaKey == metaKey);
        if (meta != null)
            meta.UpdateValue(metaValue);
        else
            _metas.Add(new BonUserMeta(Id, metaKey, metaValue));
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

    public void IncrementFailedLoginAttemptCount()
    {
        FailedLoginAttemptCount++;
        if (FailedLoginAttemptCount >= 3)
        {
            var lockDuration = TimeSpan.FromMinutes(Math.Pow(2, FailedLoginAttemptCount - 3));
            LockAccountUntil(DateTime.Now.Add(lockDuration));
        }
    }

    public void ResetFailedLoginAttemptCount()
    {
        FailedLoginAttemptCount = 0;
        AccountLockedUntil = null;
    }

    public void LockAccountUntil(DateTime until)
    {
        if (until <= DateTime.Now) throw new ArgumentException("Lock date must be in the future.", nameof(until));
        AccountLockedUntil = until;
        ChangeStatus(UserStatus.Locked);
    }

    public bool IsAccountLocked() => AccountLockedUntil.HasValue && AccountLockedUntil.Value > DateTime.Now;

    public void UpdateProfile(UserProfile newProfile) =>
        UpdatePersonalProfile(newProfile ?? throw new ArgumentNullException(nameof(newProfile)));

    public void UnlockAccount()
    {
        AccountLockedUntil = null;
        ChangeStatus(UserStatus.Active);
    }

    public void Lock(string? inputReason)
    {
        if (Status == UserStatus.Locked) throw new InvalidOperationException("User is already locked.");
        ChangeStatus(UserStatus.Locked);
        if (!string.IsNullOrEmpty(inputReason)) AddClaim("LockReason", inputReason);
        AddClaim("LockedAt", DateTime.UtcNow.ToString("O"));
    }

    public void Unlock()
    {
        if (Status != UserStatus.Locked) throw new InvalidOperationException("User is not locked.");
        ChangeStatus(UserStatus.Active);
        RemoveClaimsByType("LockReason");
        RemoveClaimsByType("LockedAt");
    }

    public void SetForceChangePassword() => AddClaim("ForcePasswordChange", "true");
}
