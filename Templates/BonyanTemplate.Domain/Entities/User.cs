using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace BonyanTemplate.Domain.Entities;

public class User : BonIdentityUser
{
    protected User()
    {
    }

    public User(BonUserId id, string userName) : base(id, userName)
    {
    }
}