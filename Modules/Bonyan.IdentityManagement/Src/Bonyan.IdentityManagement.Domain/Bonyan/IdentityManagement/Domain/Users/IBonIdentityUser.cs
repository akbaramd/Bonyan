using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.UserManagement.Domain.Users;

namespace Bonyan.IdentityManagement.Domain.Users;

public interface IBonIdentityUser : IBonUser
{
    BonUserPassword Password { get; }
    IReadOnlyCollection<BonIdentityUserToken> Tokens { get; }

    
    void SetPassword(string newPassword);
    bool VerifyPassword(string plainPassword);

    void SetToken(string tokenType, string newValue, DateTime? expiration = null);
    void RemoveToken(string tokenType);
    bool IsTokenExpired(string tokenType);
}