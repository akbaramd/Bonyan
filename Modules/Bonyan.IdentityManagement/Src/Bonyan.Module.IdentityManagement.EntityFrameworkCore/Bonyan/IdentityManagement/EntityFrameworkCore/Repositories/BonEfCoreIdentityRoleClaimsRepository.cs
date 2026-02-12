using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.Repositories;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreIdentityRoleClaimsRepository : EfCoreBonRepository<BonIdentityRoleClaims, BonRoleClaimId, IBonIdentityManagementDbContext>, IBonIdentityRoleClaimsRepository
{
    protected override IQueryable<BonIdentityRoleClaims> PrepareQuery(DbSet<BonIdentityRoleClaims> dbSet) =>
        base.PrepareQuery(dbSet).Include(x => x.Role);
}

public class BonEfCoreIdentityRoleClaimsReadOnlyRepository : EfCoreReadonlyRepository<BonIdentityRoleClaims, BonRoleClaimId, IBonIdentityManagementDbContext>, IBonIdentityRoleClaimsReadOnlyRepository
{
    protected override IQueryable<BonIdentityRoleClaims> PrepareQuery(DbSet<BonIdentityRoleClaims> dbSet) =>
        base.PrepareQuery(dbSet).Include(x => x.Role);
}
