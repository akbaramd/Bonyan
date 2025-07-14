using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Layer.Domain;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreIdentityUserRepository<TUser, TRole> : EfCoreBonRepository<TUser, BonUserId, IBonIdentityManagementDbContext<TUser, TRole>>, IBonIdentityUserRepository<TUser, TRole> 
    where TUser : BonIdentityUser<TUser, TRole> 
    where TRole : BonIdentityRole<TRole>
{
    
    public new IBonDbContextProvider<IBonIdentityManagementDbContext<TUser, TRole>> BonDbContextProvider => LazyServiceProvider.LazyGetRequiredService<IBonDbContextProvider<IBonIdentityManagementDbContext<TUser, TRole>>>();
    protected override IQueryable<TUser> PrepareQuery(DbSet<TUser> dbSet)
    {
        return base.PrepareQuery(dbSet)
            .Include(x=>x.Tokens)
            .Include(x=>x.UserRoles)
            .ThenInclude(x=>x.Role)
            .Include(x=>x.UserClaims);
    }
}

public class BonEfCoreIdentityUserReadOnlyRepository<TUser, TRole> : EfCoreReadonlyRepository<TUser, BonUserId, IBonIdentityManagementDbContext<TUser, TRole>>, IBonIdentityUserReadOnlyRepository<TUser, TRole> 
    where TUser : BonIdentityUser<TUser, TRole> 
    where TRole : BonIdentityRole<TRole>
{
    public new IBonDbContextProvider<IBonIdentityManagementDbContext<TUser, TRole>> BonDbContextProvider => LazyServiceProvider.LazyGetRequiredService<IBonDbContextProvider<IBonIdentityManagementDbContext<TUser, TRole>>>();
    protected override IQueryable<TUser> PrepareQuery(DbSet<TUser> dbSet)
    {
        return base.PrepareQuery(dbSet)
            .Include(x=>x.Tokens)
            .Include(x=>x.UserRoles)
            .ThenInclude(x=>x.Role)
            .Include(x=>x.UserClaims);
        
    }
}


