using Bonyan.Layer.Domain.Abstractions;

namespace Bonyan.IdentityManagement.Domain;

public interface IPermissionBonRepository<TPermission> : IBonRepository<TPermission> where TPermission : BonPermission
{
        
}