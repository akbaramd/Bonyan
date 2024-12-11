using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Permissions.Repositories;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonIdentityEfCorePermissionRepository<TUser> : EfCoreBonRepository<BonIdentityPermission,BonPermissionId, IBonIdentityManagementDbContext<TUser>>
    , IBonIdentityPermissionRepository
     where TUser : BonIdentityUser
{
   
}

public class BonIdentityEfCorePermissionReadonlyRepository<TUser> : EfCoreReadonlyRepository<BonIdentityPermission,BonPermissionId, IBonIdentityManagementDbContext<TUser>>
    , IBonIdentityPermissionReadOnlyRepository where TUser : BonIdentityUser
{
   
}