using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Abstractions.Roles;

public interface IBonRoleRepository : IBonRepository<BonIdentityRole> 
{
}

public interface IBonRoleReadOnlyRepository: IBonReadOnlyRepository<BonIdentityRole> 
{
}


