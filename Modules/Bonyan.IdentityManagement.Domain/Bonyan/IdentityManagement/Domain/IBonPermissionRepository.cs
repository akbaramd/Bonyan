using Bonyan.Layer.Domain.Abstractions;

namespace Bonyan.IdentityManagement.Domain;

public interface IBonPermissionRepository<TPermission> : IRepository<TPermission> where TPermission : BonPermission
{
        
}