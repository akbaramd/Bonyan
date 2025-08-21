using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.Novino.Domain.Entities;

public class User : BonIdentityUser<User, Role>
{
    public User()
    {
        
    }
    
    public User(BonUserId newId, string admin, UserProfile adminProfile) : base(newId, admin, adminProfile)
    {
    }
}