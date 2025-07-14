using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Roles.Repostories;

public interface IBonIdentityRoleClaimsRepository<TRole> : IBonRepository<BonIdentityRoleClaims<TRole>>
    where TRole : BonIdentityRole<TRole>
{
}

public interface IBonIdentityRoleClaimsReadOnlyRepository<TRole> : IBonReadOnlyRepository<BonIdentityRoleClaims<TRole>>
    where TRole : BonIdentityRole<TRole>
{
} 