using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Roles.Repositories;

/// <summary>
/// Repository for identity role aggregate (non-generic).
/// </summary>
public interface IBonIdentityRoleRepository : IBonRepository<BonIdentityRole>
{
}

/// <summary>
/// Read-only repository for identity roles. Extends framework read-only repo so app service base can resolve it.
/// </summary>
public interface IBonIdentityRoleReadOnlyRepository : IBonReadOnlyRepository<BonIdentityRole, BonRoleId>
{
}
