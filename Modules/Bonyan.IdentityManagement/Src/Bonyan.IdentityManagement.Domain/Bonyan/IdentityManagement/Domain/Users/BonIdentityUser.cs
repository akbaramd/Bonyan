using Bonyan.IdentityManagement.Domain.Users.DomainEvents;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users;

public class BonIdentityUser : BonUser, IBonIdentityUser
{
    public BonUserPassword Password { get; private set; }

    private readonly List<BonIdentityUserToken> _tokens = new List<BonIdentityUserToken>();
    public IReadOnlyCollection<BonIdentityUserToken> Tokens => _tokens;

    // Parameterless constructor for EF Core
    protected BonIdentityUser() { }

    public BonIdentityUser(BonUserId id, string userName) : base(id, userName) { }

    public BonIdentityUser(BonUserId id, string userName, string password) : base(id, userName)
    {
        SetPassword(password);
    }

    public BonIdentityUser(BonUserId id, string userName, string password, BonUserEmail? email, BonUserPhoneNumber? phoneNumber)
        : base(id, userName, email, phoneNumber)
    {
        SetPassword(password);
    }

    public void SetPassword(string newPassword)
    {
        Password = new BonUserPassword(newPassword);
        AddDomainEvent(new BonIdentityUserPasswordChangedDomainEvent(this));
    }

    public bool VerifyPassword(string password)
    {
        return Password.Verify(password);
    }

    public void SetToken(string tokenType, string newValue, DateTime? expiration = null)
    {
        if (string.IsNullOrWhiteSpace(tokenType)) 
            throw new ArgumentException("Token type cannot be empty.", nameof(tokenType));
        if (string.IsNullOrWhiteSpace(newValue)) 
            throw new ArgumentException("Token value cannot be empty.", nameof(newValue));

        // Find the token by type
        var token = _tokens.FirstOrDefault(t => t.Type == tokenType);

        if (token != null)
        {
            // Update the existing token
            token.UpdateValue(newValue, expiration);
            AddDomainEvent(new BonIdentityUserTokenUpdatedDomainEvent(this, token));
        }
        else
        {
            // Add a new token if it doesn't exist
            token = new BonIdentityUserToken(Id, tokenType, newValue, expiration);
            _tokens.Add(token);
            AddDomainEvent(new BonIdentityUserTokenAddedDomainEvent(this, token));
        }
    }



    public void RemoveToken(string tokenType)
    {
        var token = _tokens.FirstOrDefault(t => t.Type == tokenType);
        if (token == null)
        {
            throw new InvalidOperationException($"No token of type {tokenType} exists for this user.");
        }

        _tokens.Remove(token);
        AddDomainEvent(new BonIdentityUserTokenRemovedDomainEvent(this, token));
    }

    public bool IsTokenExpired(string tokenType)
    {
        var token = _tokens.FirstOrDefault(t => t.Type == tokenType);
        if (token == null)
        {
            throw new InvalidOperationException($"No token of type {tokenType} exists for this user.");
        }

        return token.IsExpired();
    }
}
