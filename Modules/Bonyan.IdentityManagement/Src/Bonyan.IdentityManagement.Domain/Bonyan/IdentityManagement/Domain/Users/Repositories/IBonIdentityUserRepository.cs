using Bonyan.UserManagement.Domain.Users.Repositories;

namespace Bonyan.IdentityManagement.Domain.Users.Repositories;

public interface IBonIdentityUserRepository<TUser> : IBonUserRepository<TUser> where TUser : class, IBonIdentityUser
{
}

public interface IBonIdentityUserReadOnlyRepository<TUser>: IBonUserReadOnlyRepository<TUser> where TUser : class, IBonIdentityUser
{
}


