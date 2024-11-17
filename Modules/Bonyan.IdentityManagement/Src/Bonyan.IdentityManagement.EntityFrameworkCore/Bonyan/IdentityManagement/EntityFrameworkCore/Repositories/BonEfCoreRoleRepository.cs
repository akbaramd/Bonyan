using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Abstractions.Roles;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonIdentityEfCoreRoleRepository<TUser> : EfCoreBonRepository<BonIdentityRole,BonRoleId,BonIdentityManagementDbContext<TUser>>,IBonIdentityRoleRepository,IBonIdentityRoleReadOnlyRepository where TUser : class, IBonIdentityUser
{
  
}