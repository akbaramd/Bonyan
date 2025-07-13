using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.IdentityManagement.Domain.Users.ValueObjects;
using Bonyan.Layer.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreIdentityUserClaimsRepository<TUser> : EfCoreBonRepository<BonIdentityUserClaims, BonUserClaimId, IBonIdentityManagementDbContext<TUser>>, IBonIdentityUserClaimsRepository where TUser : BonIdentityUser
{
    protected override IQueryable<BonIdentityUserClaims> PrepareQuery(DbSet<BonIdentityUserClaims> dbSet)
    {
        return base.PrepareQuery(dbSet)
            .Include(x => x.User);
    }
}

public class BonEfCoreIdentityUserClaimsReadOnlyRepository<TUser> : EfCoreReadonlyRepository<BonIdentityUserClaims, BonUserClaimId, IBonIdentityManagementDbContext<TUser>>, IBonIdentityUserClaimsReadOnlyRepository where TUser : BonIdentityUser
{
    protected override IQueryable<BonIdentityUserClaims> PrepareQuery(DbSet<BonIdentityUserClaims> dbSet)
    {
        return base.PrepareQuery(dbSet)
            .Include(x => x.User);
    }
} 