using Bonyan.UserManagement.Domain.Users.DomainServices;
using Bonyan.UserManagement.Domain.Users.Entities;

namespace Bonyan.IdentityManagement.Domain.Users;

public interface IBonIdentityUserManager<TIdentityUser> : IBonUserManager<TIdentityUser> where TIdentityUser : IBonIdentityUser
{
    
}

