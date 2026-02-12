using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Roles.Repositories;

/// <summary>
/// Repository for role claims (non-generic).
/// </summary>
public interface IBonIdentityRoleClaimsRepository : IBonRepository<BonIdentityRoleClaims>
{
}

/// <summary>
/// Read-only repository for role claims.
/// </summary>
public interface IBonIdentityRoleClaimsReadOnlyRepository : IBonReadOnlyRepository<BonIdentityRoleClaims>
{
}
