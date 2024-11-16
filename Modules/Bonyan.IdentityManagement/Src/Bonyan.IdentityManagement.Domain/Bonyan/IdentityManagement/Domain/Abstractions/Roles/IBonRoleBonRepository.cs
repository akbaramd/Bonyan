using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Abstractions.Roles;

public interface IBonIdentityRoleRepository : IBonRepository<BonIdentityRole> 
{
}

public interface IBonIdentityRoleReadOnlyRepository: IBonReadOnlyRepository<BonIdentityRole> 
{
}


