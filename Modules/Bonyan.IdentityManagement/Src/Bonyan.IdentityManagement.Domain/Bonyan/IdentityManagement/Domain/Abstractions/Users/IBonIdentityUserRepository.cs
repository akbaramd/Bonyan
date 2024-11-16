using Bonyan.IdentityManagement.Domain.Users;

namespace Bonyan.IdentityManagement.Domain.Abstractions.Users;

public interface IBonIdentityUserRepository : IBonIdentityUserRepository<BonIdentityUser>
{
}

public interface IBonIdentityUserReadOnlyRepository: IBonIdentityUserReadOnlyRepository<BonIdentityUser> 
{
}


