using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.ValueObjects;

namespace BonyanTemplate.Domain.Entities;

public class User : BonUser
{
    protected User()
    {
    }

    public User(BonUserId id, string userName) : base(id, userName)
    {
    }
}