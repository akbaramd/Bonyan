using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users.DomainEvents;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users;

/// <summary>
/// Represents an identity user in the domain.
/// Implements DDD principles and encapsulates behaviors for roles, tokens, and password management.
/// </summary>
public class BonIdentityUser<TUser,TRole> : BonUser where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
    private readonly List<BonIdentityUserToken<TUser,TRole>> _tokens = new List<BonIdentityUserToken<TUser,TRole>>();
    private readonly List<BonIdentityUserRoles<TUser,TRole>> _roles = new List<BonIdentityUserRoles<TUser,TRole>>();
    private readonly List<BonIdentityUserClaims<TUser,TRole>> _claims = new List<BonIdentityUserClaims<TUser,TRole>>();

    // Properties
    public BonUserPassword Password { get; private set; }
    public bool CanBeDeleted { get; private set; } = true;
    public DateTime? BannedUntil { get; private set; }
    public int FailedLoginAttemptCount { get; private set; }
    public DateTime? AccountLockedUntil { get; private set; }
    public UserProfile Profile { get; private set; }

    public IReadOnlyCollection<BonIdentityUserToken<TUser,TRole>> Tokens => _tokens;
    public IReadOnlyCollection<BonIdentityUserRoles<TUser,TRole>> UserRoles => _roles;
    public IReadOnlyCollection<BonIdentityUserClaims<TUser,TRole>> UserClaims => _claims;

    // Parameterless constructor for EF Core
    protected BonIdentityUser() { }

    /// <summary>
    /// Constructs a new identity user with required fields.
    /// </summary>
    public BonIdentityUser(BonUserId id, string userName, UserProfile profile) : base(id, userName)
    {
        Profile = profile ?? throw new ArgumentNullException(nameof(profile));
    }

    /// <summary>
    /// Constructs a new identity user with a password.
    /// </summary>
    public BonIdentityUser(BonUserId id, string userName, string password, UserProfile profile)
        : this(id, userName, profile)
    {
        SetPassword(password);
    }

    /// <summary>
    /// Constructs a new identity user with additional fields.
    /// </summary>
    public BonIdentityUser(BonUserId id, string userName, string password, BonUserEmail email, BonUserPhoneNumber phoneNumber, UserProfile profile)
        : this(id, userName, password, profile)
    {
        SetEmail(email);
        SetPhoneNumber(phoneNumber);
    }

    // Behavior

    /// <summary>
    /// Updates the password and triggers a domain event.
    /// </summary>
    public void SetPassword(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Password cannot be empty.", nameof(newPassword));

        Password = new BonUserPassword(newPassword);
        AddDomainEvent(new BonIdentityUserPasswordChangedDomainEvent(this));
    }

    /// <summary>
    /// Verifies the given password against the stored password.
    /// </summary>
    public bool VerifyPassword(string password)
    {
        return Password.Verify(password);
    }

    /// <summary>
    /// Sets or updates a token for the user.
    /// </summary>
    public void SetToken(string tokenType, string newValue, DateTime? expiration = null)
    {
        if (string.IsNullOrWhiteSpace(tokenType))
            throw new ArgumentException("Token type cannot be empty.", nameof(tokenType));
        if (string.IsNullOrWhiteSpace(newValue))
            throw new ArgumentException("Token value cannot be empty.", nameof(newValue));

        var token = _tokens.FirstOrDefault(t => t.Type == tokenType);

        if (token != null)
        {
            token.UpdateValue(newValue, expiration);
            AddDomainEvent(new BonIdentityUserTokenUpdatedDomainEvent(this, token));
        }
        else
        {
            token = new BonIdentityUserToken<TUser,TRole>(Id, tokenType, newValue, expiration);
            _tokens.Add(token);
            AddDomainEvent(new BonIdentityUserTokenAddedDomainEvent(this, token));
        }
    }

    /// <summary>
    /// Removes a token by its type.
    /// </summary>
    public void RemoveToken(string tokenType)
    {
        var token = _tokens.FirstOrDefault(t => t.Type == tokenType);
        if (token == null)
            throw new InvalidOperationException($"No token of type {tokenType} exists for this user.");

        _tokens.Remove(token);
        AddDomainEvent(new BonIdentityUserTokenRemovedDomainEvent(this, token));
    }

    /// <summary>
    /// Checks if a specific token has expired.
    /// </summary>
    public bool IsTokenExpired(string tokenType)
    {
        var token = _tokens.FirstOrDefault(t => t.Type == tokenType);
        if (token == null)
            throw new InvalidOperationException($"No token of type {tokenType} exists for this user.");

        return token.IsExpired();
    }

    /// <summary>
    /// Assigns a role to the user.
    /// </summary>
    public void AssignRole(BonRoleId roleId)
    {
        if (_roles.Any(r => r.RoleId == roleId))
            throw new InvalidOperationException($"User already has the role with ID {roleId.Value}.");

        var role = new BonIdentityUserRoles<TUser,TRole>(Id, roleId);
        _roles.Add(role);
        AddDomainEvent(new BonIdentityUserRoleAddedDomainEvent(this, roleId));
    }

    /// <summary>
    /// Removes a role from the user.
    /// </summary>
    public void RemoveRole(BonRoleId roleId)
    {
        var role = _roles.FirstOrDefault(r => r.RoleId == roleId);
        if (role == null)
            throw new InvalidOperationException($"User does not have the role with ID {roleId.Value}.");

        _roles.Remove(role);
        AddDomainEvent(new BonIdentityUserRoleRemovedDomainEvent(this, roleId));
    }

    /// <summary>
    /// Adds a claim to the user.
    /// </summary>
    public void AddClaim(BonUserClaimId claimId, string claimType, string claimValue, string? issuer = null)
    {
        if (claimId == null)
            throw new ArgumentNullException(nameof(claimId));
        if (string.IsNullOrEmpty(claimType))
            throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));
        if (string.IsNullOrEmpty(claimValue))
            throw new ArgumentException("Claim value cannot be null or empty.", nameof(claimValue));

        var existingClaim = _claims.FirstOrDefault(c => c.ClaimType == claimType && c.ClaimValue == claimValue);
        if (existingClaim == null)
        {
            var claim = new BonIdentityUserClaims<TUser,TRole>(claimId, Id, claimType, claimValue, null, issuer);
            _claims.Add(claim);
        }
    }

    /// <summary>
    /// Removes a claim from the user.
    /// </summary>
    public void RemoveClaim(string claimType, string claimValue)
    {
        if (string.IsNullOrEmpty(claimType))
            throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));
        if (string.IsNullOrEmpty(claimValue))
            throw new ArgumentException("Claim value cannot be null or empty.", nameof(claimValue));

        var userClaim = _claims.FirstOrDefault(c => c.ClaimType == claimType && c.ClaimValue == claimValue);
        if (userClaim != null)
        {
            _claims.Remove(userClaim);
        }
    }

    /// <summary>
    /// Removes all claims of a specific type from the user.
    /// </summary>
    public void RemoveClaimsByType(string claimType)
    {
        if (string.IsNullOrEmpty(claimType))
            throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));

        var claimsToRemove = _claims.Where(c => c.ClaimType == claimType).ToList();
        foreach (var claim in claimsToRemove)
        {
            _claims.Remove(claim);
        }
    }

    /// <summary>
    /// Checks if the user has a specific claim.
    /// </summary>
    public bool HasClaim(string claimType, string claimValue)
    {
        if (string.IsNullOrEmpty(claimType))
            throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));
        if (string.IsNullOrEmpty(claimValue))
            throw new ArgumentException("Claim value cannot be null or empty.", nameof(claimValue));

        return _claims.Any(c => c.ClaimType == claimType && c.ClaimValue == claimValue);
    }

    /// <summary>
    /// Gets all claims of a specific type.
    /// </summary>
    public IEnumerable<BonIdentityUserClaims<TUser,TRole>> GetClaimsByType(string claimType)
    {
        if (string.IsNullOrEmpty(claimType))
            throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));

        return _claims.Where(c => c.ClaimType == claimType);
    }

    /// <summary>
    /// Marks the user as non-deletable.
    /// </summary>
    public void MarkAsNonDeletable()
    {
        CanBeDeleted = false;
    }

    /// <summary>
    /// Marks the user as deletable.
    /// </summary>
    public void MarkAsDeletable()
    {
        CanBeDeleted = true;
    }

    /// <summary>
    /// Increments the failed login attempt count and locks the account if necessary.
    /// </summary>
    public void IncrementFailedLoginAttemptCount()
    {
        FailedLoginAttemptCount++;

        if (FailedLoginAttemptCount >= 3)
        {
            var lockDuration = TimeSpan.FromMinutes(Math.Pow(2, FailedLoginAttemptCount - 3)); // 1, 2, 4, 8, ... minutes
            LockAccountUntil(DateTime.Now.Add(lockDuration));
        }
    }

    /// <summary>
    /// Resets the failed login attempt count and unlocks the account.
    /// </summary>
    public void ResetFailedLoginAttemptCount()
    {
        FailedLoginAttemptCount = 0;
        AccountLockedUntil = null;
    }

    /// <summary>
    /// Locks the account until a specified date.
    /// </summary>
    public void LockAccountUntil(DateTime until)
    {
        if (until <= DateTime.Now)
            throw new ArgumentException("Lock date must be in the future.", nameof(until));

        AccountLockedUntil = until;
        ChangeStatus(UserStatus.Locked);
    }

    /// <summary>
    /// Checks if the account is currently locked.
    /// </summary>
    public bool IsAccountLocked()
    {
        return AccountLockedUntil.HasValue && AccountLockedUntil.Value > DateTime.Now;
    }

    /// <summary>
    /// Method to update the profile
    /// </summary>
    public void UpdateProfile(UserProfile newProfile)
    {
        Profile = newProfile ?? throw new ArgumentNullException(nameof(newProfile));
    }

    /// <summary>
    /// Unlocks the account by resetting the lock status and lockout date.
    /// </summary>
    public void UnlockAccount()
    {
        AccountLockedUntil = null;
        ChangeStatus(UserStatus.Active); // Assuming UserStatus.Active is the status for an unlocked account
    }

}
