using Bonyan.Layer.Domain.Abstractions;

namespace Bonyan.IdentityManagement.Domain;

public interface IBonRoleRepository : IBonRepository<BonIdentityRole> 
{
}

public interface IBonRoleReadOnlyRepository: IBonReadOnlyRepository<BonIdentityRole> 
{
}


