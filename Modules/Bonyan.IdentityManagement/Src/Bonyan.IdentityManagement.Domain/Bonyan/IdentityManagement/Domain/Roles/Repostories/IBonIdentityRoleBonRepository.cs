using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Roles.Repositories;

public interface IBonIdentityRoleRepository : IBonRepository<BonIdentityRole> 
{
}

public interface IBonIdentityRoleReadOnlyRepository: IBonReadOnlyRepository<BonIdentityRole> 
{
}


