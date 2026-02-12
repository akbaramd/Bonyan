using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Users.Repositories;

/// <summary>
/// Repository for the final identity user aggregate (non-generic).
/// </summary>
public interface IBonIdentityUserRepository : IBonRepository<BonIdentityUser>
{
}

/// <summary>
/// Read-only repository for identity users.
/// </summary>
public interface IBonIdentityUserReadOnlyRepository : IBonReadOnlyRepository<BonIdentityUser>
{
}
