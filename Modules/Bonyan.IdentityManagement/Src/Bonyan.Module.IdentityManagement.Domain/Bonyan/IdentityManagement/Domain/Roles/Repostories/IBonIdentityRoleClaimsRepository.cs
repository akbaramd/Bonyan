using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Roles.Repostories;

public interface IBonIdentityRoleClaimsRepository : IBonRepository<BonIdentityRoleClaims>
{
}

public interface IBonIdentityRoleClaimsReadOnlyRepository : IBonReadOnlyRepository<BonIdentityRoleClaims>
{
} 