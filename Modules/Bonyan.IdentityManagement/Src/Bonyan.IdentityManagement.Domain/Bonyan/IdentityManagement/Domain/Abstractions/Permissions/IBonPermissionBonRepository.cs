using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Abstractions.Permissions;

public interface IBonIdentityPermissionRepository : IBonRepository<BonIdentityPermission>
{
        
}public interface IBonIdentityPermissionReadOnlyRepository : IBonReadOnlyRepository<BonIdentityPermission>
{
        
}