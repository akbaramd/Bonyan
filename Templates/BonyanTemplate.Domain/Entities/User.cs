using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.ValueObjects;

namespace BonyanTemplate.Domain.Entities;

public class User : BonyanUser
{
    protected User()
    {
    }

    public User(UserId id, string userName) : base(id, userName)
    {
    }
}