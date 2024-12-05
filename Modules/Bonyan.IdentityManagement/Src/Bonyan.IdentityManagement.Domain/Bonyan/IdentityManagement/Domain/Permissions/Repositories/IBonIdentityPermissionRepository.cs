using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Permissions.Repositories;

public interface IBonIdentityPermissionRepository : IBonRepository<BonIdentityPermission,BonPermissionId>
{
        
}public interface IBonIdentityPermissionReadOnlyRepository : IBonReadOnlyRepository<BonIdentityPermission,BonPermissionId>
{
        
}