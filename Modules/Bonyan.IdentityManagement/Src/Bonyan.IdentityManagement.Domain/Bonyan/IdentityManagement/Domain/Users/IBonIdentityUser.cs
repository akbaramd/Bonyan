using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.UserManagement.Domain.Users;

namespace Bonyan.IdentityManagement.Domain.Users;

public interface IBonIdentityUser : IBonUser
{
    BonUserPassword Password { get; }
    
    void SetPassword(string newPassword);
    bool VerifyPassword(string plainPassword);
}