using Bonyan.Layer.Domain.Abstractions;

namespace Bonyan.IdentityManagement.Domain;

public interface IBonRoleRepository<TRole> : IRepository<TRole> where TRole : BonRole
{
}