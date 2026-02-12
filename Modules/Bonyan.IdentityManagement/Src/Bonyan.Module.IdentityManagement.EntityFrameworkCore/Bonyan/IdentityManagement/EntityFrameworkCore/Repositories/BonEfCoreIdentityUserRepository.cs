using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Layer.Domain;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreIdentityUserRepository : EfCoreBonRepository<BonIdentityUser, BonUserId, IBonIdentityManagementDbContext>, IBonIdentityUserRepository
{
    public new IBonDbContextProvider<IBonIdentityManagementDbContext> BonDbContextProvider =>
        LazyServiceProvider.LazyGetRequiredService<IBonDbContextProvider<IBonIdentityManagementDbContext>>();

    protected override IQueryable<BonIdentityUser> PrepareQuery(DbSet<BonIdentityUser> dbSet) =>
        base.PrepareQuery(dbSet)
            .Include(x => x.Tokens)
            .Include(x => x.UserRoles)
            .Include(x => x.UserClaims);
}

public class BonEfCoreIdentityUserReadOnlyRepository : EfCoreReadonlyRepository<BonIdentityUser, BonUserId, IBonIdentityManagementDbContext>, IBonIdentityUserReadOnlyRepository
{
    public new IBonDbContextProvider<IBonIdentityManagementDbContext> BonDbContextProvider =>
        LazyServiceProvider.LazyGetRequiredService<IBonDbContextProvider<IBonIdentityManagementDbContext>>();

    protected override IQueryable<BonIdentityUser> PrepareQuery(DbSet<BonIdentityUser> dbSet) =>
        base.PrepareQuery(dbSet)
            .Include(x => x.Tokens)
            .Include(x => x.UserRoles)
            .Include(x => x.UserClaims);
}
