using Bonyan.Layer.Domain.Abstractions;

namespace Bonyan.IdentityManagement.Domain;

public interface IBonPermissionRepository : IBonRepository<BonIdentityPermission>
{
        
}public interface IBonPermissionReadOnlyRepository : IBonReadOnlyRepository<BonIdentityPermission>
{
        
}