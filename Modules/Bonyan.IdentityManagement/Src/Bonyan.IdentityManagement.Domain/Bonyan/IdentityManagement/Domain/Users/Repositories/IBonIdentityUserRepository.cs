using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Users.Repositories;

public interface IBonIdentityUserRepository<TUser> : IBonRepository<TUser> where TUser : BonIdentityUser
{
}

public interface IBonIdentityUserReadOnlyRepository<TUser>: IBonReadOnlyRepository<TUser> where TUser : BonIdentityUser
{
}


