using Bonyan.IdentityManagement.Domain;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.ValueObjects;

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