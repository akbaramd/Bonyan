using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Users.Repositories;

public interface IBonIdentityUserRolesRepository<TUser,TRole> : IBonRepository<BonIdentityUserRoles<TUser,TRole>>
    where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
}

