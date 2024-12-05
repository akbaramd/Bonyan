using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Users.Repositories;

public interface IBonIdentityUserRepository<TUser> : IBonRepository<TUser> where TUser : class, IBonIdentityUser
{
}

public interface IBonIdentityUserReadOnlyRepository<TUser>: IBonReadOnlyRepository<TUser> where TUser : class, IBonIdentityUser
{
}


