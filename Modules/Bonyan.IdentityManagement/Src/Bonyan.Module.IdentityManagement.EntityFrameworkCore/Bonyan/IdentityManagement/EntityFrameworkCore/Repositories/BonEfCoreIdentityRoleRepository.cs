using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.Repositories;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonIdentityEfCoreRoleRepository : EfCoreBonRepository<BonIdentityRole, BonRoleId, IBonIdentityManagementDbContext>,
    IBonIdentityRoleRepository, IBonIdentityRoleReadOnlyRepository
{
    protected override IQueryable<BonIdentityRole> PrepareQuery(DbSet<BonIdentityRole> dbSet) =>
        base.PrepareQuery(dbSet).Include(x => x.RoleClaims);
}
