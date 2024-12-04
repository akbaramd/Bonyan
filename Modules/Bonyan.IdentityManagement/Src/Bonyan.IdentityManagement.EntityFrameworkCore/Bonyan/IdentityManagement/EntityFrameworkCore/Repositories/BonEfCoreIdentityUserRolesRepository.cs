using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Layer.Domain;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreIdentityUserRolesRepository<TUser> : EfCoreBonRepository<BonIdentityUserRoles,IBonIdentityManagementDbContext<TUser>>,IBonIdentityUserRolesRepository where TUser : class, IBonIdentityUser
{
  
}