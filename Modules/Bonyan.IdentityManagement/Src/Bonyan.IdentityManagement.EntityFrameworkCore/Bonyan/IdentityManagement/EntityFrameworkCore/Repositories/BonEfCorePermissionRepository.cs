using Bonyan.IdentityManagement.Domain;
using Bonyan.Layer.Domain;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCorePermissionRepository : EfCoreBonRepository<BonIdentityPermission, BonIdentityManagementDbContext>
    , IBonPermissionRepository
    , IBonPermissionReadOnlyRepository
{
    public BonEfCorePermissionRepository(BonIdentityManagementDbContext userManagementDbContext) : base(
        userManagementDbContext)
    {
    }
}