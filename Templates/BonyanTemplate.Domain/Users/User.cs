using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace BonyanTemplate.Domain.Users;

public class User : BonIdentityUser<User,Role>
{
    protected User()
    {
    }

    public User(BonUserId id, string userName) : base(id, userName,new UserProfile("","",DateTime.Now,""))
    {
    }
}