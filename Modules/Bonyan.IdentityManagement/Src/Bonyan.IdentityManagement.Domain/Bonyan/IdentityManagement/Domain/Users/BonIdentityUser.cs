using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users.DomainEvents;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users;

/// <summary>
/// Represents an identity user in the domain.
/// Implements DDD principles and encapsulates behaviors for roles, tokens, and password management.
/// </summary>
public class BonIdentityUser : BonUser, IBonIdentityUser
{
    private readonly List<BonIdentityUserToken> _tokens = new List<BonIdentityUserToken>();
    private readonly List<BonIdentityUserRoles> _roles = new List<BonIdentityUserRoles>();

    // Properties
    public BonUserPassword Password { get; private set; }
    public bool CanBeDeleted { get; private set; } = true;

    public IReadOnlyCollection<BonIdentityUserToken> Tokens => _tokens;
    public IReadOnlyCollection<BonIdentityUserRoles> UserRoles => _roles;

    // Parameterless constructor for EF Core
    protected BonIdentityUser() { }

    /// <summary>
    /// Constructs a new identity user with required fields.
    /// </summary>
    public BonIdentityUser(BonUserId id, string userName) : base(id, userName) { }

    /// <summary>
    /// Constructs a new identity user with a password.
    /// </summary>
    public BonIdentityUser(BonUserId id, string userName, string password) : this(id, userName)
    {
        SetPassword(password);
    }

    /// <summary>
    /// Constructs a new identity user with additional fields.
    /// </summary>
    public BonIdentityUser(BonUserId id, string userName, string password, BonUserEmail email, BonUserPhoneNumber phoneNumber)
        : this(id, userName, password)
    {
        SetEmail(email );
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
            token = new BonIdentityUserToken(Id, tokenType, newValue, expiration);
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

        var role = new BonIdentityUserRoles(Id, roleId);
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

}
