using Bonyan.IdentityManagement.Domain;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public interface IBonIdentityManagementDbContext<TUser> : IBonUserManagementDbContext<TUser>
    where TUser : BonUser 
{
    public DbSet<BonIdentityRole> Roles { get; set; }
    public DbSet<BonIdentityPermission> Permissions { get; set; }
}

public interface IBonIdentityManagementDbContext : IBonIdentityManagementDbContext<BonIdentityUser>
{
    public DbSet<BonIdentityRole> Roles { get; set; }
    public DbSet<BonIdentityPermission> Permissions { get; set; }
}