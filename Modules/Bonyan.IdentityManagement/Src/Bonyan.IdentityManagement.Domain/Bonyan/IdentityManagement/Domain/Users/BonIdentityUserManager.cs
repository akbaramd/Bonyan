using Bonyan.IdentityManagement.Domain.Abstractions.Users;
using Bonyan.UserManagement.Domain.Users.DomainServices;

namespace Bonyan.IdentityManagement.Domain.Users
{
    public class BonIdentityUserManager<TUser> : BonUserManager<TUser> ,IBonIdentityUserManager<TUser> where TUser : class, IBonIdentityUser
    {
    }
    
    public class BonIdentityUserManager : BonIdentityUserManager<BonIdentityUser>,IBonIdentityUserManager
    {
    }
}