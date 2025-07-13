using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Users.Repositories;

public interface IBonIdentityUserClaimsRepository : IBonRepository<BonIdentityUserClaims>
{
}

public interface IBonIdentityUserClaimsReadOnlyRepository : IBonReadOnlyRepository<BonIdentityUserClaims>
{
} 