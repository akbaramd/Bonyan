using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Users.Repositories;

public interface IBonIdentityUserRepository<TUser,TRole> : IBonRepository<TUser> where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
}

public interface IBonIdentityUserReadOnlyRepository<TUser,TRole>: IBonReadOnlyRepository<TUser> 
    where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
}


