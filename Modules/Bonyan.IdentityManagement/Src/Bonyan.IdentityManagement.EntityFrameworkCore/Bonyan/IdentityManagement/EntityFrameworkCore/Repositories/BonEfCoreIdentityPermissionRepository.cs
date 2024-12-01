using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Permissions.Repositories;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonIdentityEfCorePermissionRepository<TUser> : EfCoreBonRepository<BonIdentityPermission, IBonIdentityManagementDbContext<TUser>>
    , IBonIdentityPermissionRepository
    , IBonIdentityPermissionReadOnlyRepository where TUser : class, IBonIdentityUser
{
   
}