using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;

public class BonEfCoreIdentityUserRepository<TUser> : BonEfCoreUserRepository<TUser>,IBonIdentityUserRepository<TUser> where TUser : class, IBonIdentityUser
{
    
    public new IBonDbContextProvider<IBonIdentityManagementDbContext<TUser>> BonDbContextProvider => LazyServiceProvider.LazyGetRequiredService<IBonDbContextProvider<IBonIdentityManagementDbContext<TUser>>>();
    protected override IQueryable<TUser> PrepareQuery(DbSet<TUser> dbSet)
    {
        return base.PrepareQuery(dbSet).Include(x=>x.Tokens);
    }
}
public class BonEfCoreIdentityUserReadOnlyRepository<TUser> : BonEfCoreUserReadOnlyRepository<TUser>,IBonIdentityUserReadOnlyRepository<TUser> where TUser : class, IBonIdentityUser
{
    public new IBonDbContextProvider<IBonIdentityManagementDbContext<TUser>> BonDbContextProvider => LazyServiceProvider.LazyGetRequiredService<IBonDbContextProvider<IBonIdentityManagementDbContext<TUser>>>();
    protected override IQueryable<TUser> PrepareQuery(DbSet<TUser> dbSet)
    {
        return base.PrepareQuery(dbSet).Include(x=>x.Tokens);
        
    }
}


