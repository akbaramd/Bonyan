using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Roles.Repostories;

public interface IBonIdentityRoleRepository : IBonRepository<BonIdentityRole> 
{
}

public interface IBonIdentityRoleReadOnlyRepository: IBonReadOnlyRepository<BonIdentityRole> 
{
}


