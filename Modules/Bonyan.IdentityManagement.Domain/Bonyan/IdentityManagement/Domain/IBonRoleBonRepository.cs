using Bonyan.Layer.Domain.Repository;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain;

public interface IBonRoleRepository : IBonRepository<BonIdentityRole> 
{
}

public interface IBonRoleReadOnlyRepository: IBonReadOnlyRepository<BonIdentityRole> 
{
}


