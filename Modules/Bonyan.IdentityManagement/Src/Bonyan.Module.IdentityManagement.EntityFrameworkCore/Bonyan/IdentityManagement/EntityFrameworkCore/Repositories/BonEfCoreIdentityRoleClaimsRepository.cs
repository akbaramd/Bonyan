using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.Repostories;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreIdentityRoleClaimsRepository<TUser> : EfCoreBonRepository<BonIdentityRoleClaims, BonRoleClaimId, IBonIdentityManagementDbContext<TUser>>, IBonIdentityRoleClaimsRepository where TUser : BonIdentityUser
{
    protected override IQueryable<BonIdentityRoleClaims> PrepareQuery(DbSet<BonIdentityRoleClaims> dbSet)
    {
        return base.PrepareQuery(dbSet)
            .Include(x => x.Role);
    }
}

public class BonEfCoreIdentityRoleClaimsReadOnlyRepository<TUser> : EfCoreReadonlyRepository<BonIdentityRoleClaims, BonRoleClaimId, IBonIdentityManagementDbContext<TUser>>, IBonIdentityRoleClaimsReadOnlyRepository where TUser : BonIdentityUser
{
    protected override IQueryable<BonIdentityRoleClaims> PrepareQuery(DbSet<BonIdentityRoleClaims> dbSet)
    {
        return base.PrepareQuery(dbSet)
            .Include(x => x.Role);
    }
} 