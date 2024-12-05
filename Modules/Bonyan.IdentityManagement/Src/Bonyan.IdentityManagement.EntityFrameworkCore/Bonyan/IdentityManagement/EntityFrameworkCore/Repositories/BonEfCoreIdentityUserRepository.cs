using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.Layer.Domain;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreIdentityUserRepository<TUser> : EfCoreBonRepository<TUser,BonUserId,IBonIdentityManagementDbContext<TUser>>,IBonIdentityUserRepository<TUser> where TUser : class, IBonIdentityUser
{
    
    public new IBonDbContextProvider<IBonIdentityManagementDbContext<TUser>> BonDbContextProvider => LazyServiceProvider.LazyGetRequiredService<IBonDbContextProvider<IBonIdentityManagementDbContext<TUser>>>();
    protected override IQueryable<TUser> PrepareQuery(DbSet<TUser> dbSet)
    {
        return base.PrepareQuery(dbSet).Include(x=>x.Tokens).Include(x=>x.UserRoles);
    }
}
public class BonEfCoreIdentityUserReadOnlyRepository<TUser> : EfCoreReadonlyRepository<TUser,BonUserId,IBonIdentityManagementDbContext<TUser>>,IBonIdentityUserReadOnlyRepository<TUser> where TUser : class, IBonIdentityUser
{
    public new IBonDbContextProvider<IBonIdentityManagementDbContext<TUser>> BonDbContextProvider => LazyServiceProvider.LazyGetRequiredService<IBonDbContextProvider<IBonIdentityManagementDbContext<TUser>>>();
    protected override IQueryable<TUser> PrepareQuery(DbSet<TUser> dbSet)
    {
        return base.PrepareQuery(dbSet).Include(x=>x.Tokens).Include(x=>x.UserRoles);
        
    }
}


