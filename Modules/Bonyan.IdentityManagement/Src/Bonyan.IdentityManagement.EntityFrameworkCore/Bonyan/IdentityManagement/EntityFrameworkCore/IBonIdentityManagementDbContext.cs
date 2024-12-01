using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public interface IBonIdentityManagementDbContext<TUser> : IBonUserManagementDbContext<TUser>
    where TUser : class, IBonIdentityUser 
{
    public DbSet<BonIdentityUserToken> UserTokens { get; set; }
    public DbSet<BonIdentityRole> Roles { get; set; }
    public DbSet<BonIdentityPermission> Permissions { get; set; }
}

public interface IBonIdentityManagementDbContext : IBonIdentityManagementDbContext<BonIdentityUser>
{
}