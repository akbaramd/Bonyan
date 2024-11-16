using Bonyan.UserManagement.Domain.Users.Entities;

namespace Bonyan.IdentityManagement.Domain.Users;

public interface IBonIdentityUser : IBonUser
{
    public IReadOnlyCollection<IBonIdentityRole> Roles { get; }
}