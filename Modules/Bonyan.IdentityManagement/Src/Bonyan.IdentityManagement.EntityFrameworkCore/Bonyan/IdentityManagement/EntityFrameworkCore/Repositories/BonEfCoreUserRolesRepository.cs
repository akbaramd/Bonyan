using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Layer.Domain;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreUserRolesRepository<TUser> : EfCoreBonRepository<BonIdentityUserRoles,BonIdentityManagementDbContext<TUser>>,IBonIdentityUserRolesRepository where TUser : class, IBonIdentityUser
{
  
}