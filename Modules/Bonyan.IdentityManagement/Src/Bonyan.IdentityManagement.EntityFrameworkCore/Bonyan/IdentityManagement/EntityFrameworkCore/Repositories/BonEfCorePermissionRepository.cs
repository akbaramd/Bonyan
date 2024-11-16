using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Abstractions.Permissions;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.Layer.Domain;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonIdentityEfCorePermissionRepository : EfCoreBonRepository<BonIdentityPermission, BonIdentityManagementDbContext>
    , IBonIdentityPermissionRepository
    , IBonIdentityPermissionReadOnlyRepository
{
    public BonIdentityEfCorePermissionRepository(BonIdentityManagementDbContext userManagementDbContext) : base(
        userManagementDbContext)
    {
    }
}