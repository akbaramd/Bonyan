using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain;

public interface IBonPermissionRepository : IBonRepository<BonIdentityPermission>
{
        
}public interface IBonPermissionReadOnlyRepository : IBonReadOnlyRepository<BonIdentityPermission>
{
        
}