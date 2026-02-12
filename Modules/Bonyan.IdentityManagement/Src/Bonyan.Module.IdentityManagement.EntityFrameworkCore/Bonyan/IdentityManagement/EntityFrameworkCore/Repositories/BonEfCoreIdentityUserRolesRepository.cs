using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Layer.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreIdentityUserRolesRepository : EfCoreBonRepository<BonIdentityUserRoles, IBonIdentityManagementDbContext>, IBonIdentityUserRolesRepository
{
}
