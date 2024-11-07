using Bonyan.Layer.Domain.Abstractions;

namespace Bonyan.IdentityManagement.Domain;

public interface IBonRoleRepository<TRole> : IBonRepository<TRole> where TRole : BonRole
{
}

public interface IBonRoleReadOnlyRepository<TRole> : IBonReadOnlyRepository<TRole> where TRole : BonRole
{
}