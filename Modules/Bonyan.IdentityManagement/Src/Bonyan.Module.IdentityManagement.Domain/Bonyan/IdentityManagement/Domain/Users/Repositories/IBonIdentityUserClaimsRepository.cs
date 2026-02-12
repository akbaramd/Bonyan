using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Domain.Users.Repositories;

/// <summary>
/// Repository for user claims (non-generic).
/// </summary>
public interface IBonIdentityUserClaimsRepository 
    : IBonRepository<BonIdentityUserClaims>, IBonIdentityUserClaimsReadOnlyRepository
{
}

/// <summary>
/// Read-only repository for user claims.
/// </summary>
public interface IBonIdentityUserClaimsReadOnlyRepository : IBonReadOnlyRepository<BonIdentityUserClaims>
{
}
