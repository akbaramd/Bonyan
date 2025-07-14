using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Roles.Repostories;

public interface IBonIdentityRoleRepository<TRole> : IBonRepository<TRole> 
    where TRole : BonIdentityRole<TRole>
{
}

public interface IBonIdentityRoleReadOnlyRepository<TRole> : IBonReadOnlyRepository<TRole> 
    where TRole : BonIdentityRole<TRole>
{
}


