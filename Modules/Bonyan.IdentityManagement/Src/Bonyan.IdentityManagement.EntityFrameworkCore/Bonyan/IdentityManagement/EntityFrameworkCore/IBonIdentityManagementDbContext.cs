using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public interface IBonIdentityManagementDbContext<TUser> : IBonUserManagementDbContext<TUser>
    where TUser : BonIdentityUser 
{
    public DbSet<BonIdentityUserToken> UserTokens { get; set; }
    public DbSet<BonIdentityRole> Roles { get; set; }
    public DbSet<BonIdentityPermission> Permissions { get; set; }
    public DbSet<BonIdentityUserRoles> UserRoles { get; set; }
    public DbSet<BonIdentityRolePermissions> RolePermissions { get; set; }
}

public interface IBonIdentityManagementDbContext : IBonIdentityManagementDbContext<BonIdentityUser>
{
}

