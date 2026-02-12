using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.Layer.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreIdentityUserClaimsRepository : EfCoreBonRepository<BonIdentityUserClaims, BonUserClaimId, IBonIdentityManagementDbContext>, IBonIdentityUserClaimsRepository
{
}

public class BonEfCoreIdentityUserClaimsReadOnlyRepository : EfCoreReadonlyRepository<BonIdentityUserClaims, BonUserClaimId, IBonIdentityManagementDbContext>, IBonIdentityUserClaimsReadOnlyRepository
{
}
