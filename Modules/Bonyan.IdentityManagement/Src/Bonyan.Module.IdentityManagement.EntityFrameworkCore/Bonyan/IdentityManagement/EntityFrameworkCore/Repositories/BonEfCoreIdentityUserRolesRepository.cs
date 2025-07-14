using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Layer.Domain;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreIdentityUserRolesRepository<TUser, TRole> : EfCoreBonRepository<BonIdentityUserRoles<TUser, TRole>, IBonIdentityManagementDbContext<TUser, TRole>>, IBonIdentityUserRolesRepository<TUser, TRole> 
    where TUser : BonIdentityUser<TUser, TRole>
    where TRole : BonIdentityRole<TRole>
{
  
}