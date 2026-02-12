using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Users.Repositories;

/// <summary>
/// Repository for user-roles (non-generic).
/// </summary>
public interface IBonIdentityUserRolesRepository : IBonRepository<BonIdentityUserRoles>
{
}
