using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.Repostories;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreIdentityRoleClaimsRepository<TUser,TRole> : EfCoreBonRepository<BonIdentityRoleClaims<TRole>, BonRoleClaimId, IBonIdentityManagementDbContext<TUser,TRole>>, IBonIdentityRoleClaimsRepository<TRole> 
    where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
    protected override IQueryable<BonIdentityRoleClaims<TRole>> PrepareQuery(DbSet<BonIdentityRoleClaims<TRole>> dbSet)
    {
        return base.PrepareQuery(dbSet)
            .Include(x => x.Role);
    }
}

public class BonEfCoreIdentityRoleClaimsReadOnlyRepository<TUser,TRole> : 
    EfCoreReadonlyRepository<BonIdentityRoleClaims<TRole>, BonRoleClaimId, 
        IBonIdentityManagementDbContext<TUser,TRole>>, IBonIdentityRoleClaimsReadOnlyRepository<TRole> 
    where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
    protected override IQueryable<BonIdentityRoleClaims<TRole>> PrepareQuery(DbSet<BonIdentityRoleClaims<TRole>> dbSet)
    {
        return base.PrepareQuery(dbSet)
            .Include(x => x.Role);
    }
} 