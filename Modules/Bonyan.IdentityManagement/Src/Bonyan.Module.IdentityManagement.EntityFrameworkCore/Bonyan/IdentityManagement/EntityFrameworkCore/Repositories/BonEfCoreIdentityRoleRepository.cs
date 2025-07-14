using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.Repostories;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class
    BonIdentityEfCoreRoleRepository<TUser, TRole> :
    EfCoreBonRepository<TRole, BonRoleId, IBonIdentityManagementDbContext<TUser, TRole>>,
    IBonIdentityRoleRepository<TRole>, IBonIdentityRoleReadOnlyRepository<TRole> 
    where TUser : BonIdentityUser<TUser, TRole>
    where TRole : BonIdentityRole<TRole>
{
    protected override IQueryable<TRole> PrepareQuery(DbSet<TRole> dbSet)
    {
        return base.PrepareQuery(dbSet)
            .Include(x=>x.RoleClaims);
    }
}