using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Permissions.Repositories;

public interface IBonIdentityPermissionRepository : IBonRepository<BonIdentityPermission>
{
        
}public interface IBonIdentityPermissionReadOnlyRepository : IBonReadOnlyRepository<BonIdentityPermission>
{
        
}