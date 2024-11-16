using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public class BonIdentityManagementDbContext<TUser> :
    BonDbContext<BonIdentityManagementDbContext<TUser>>
    , IBonIdentityManagementDbContext<TUser> where TUser :class,  IBonIdentityUser
{
    public BonIdentityManagementDbContext(DbContextOptions<BonIdentityManagementDbContext<TUser>> options) :
        base(options)
    {
    }

    public DbSet<TUser> Users { get; set; }
    public DbSet<BonIdentityRole> Roles { get; set; }
    public DbSet<BonIdentityPermission> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureBonIdentityManagementByConvention<TUser>();
        base.OnModelCreating(modelBuilder);
    }
}


public class BonIdentityManagementDbContext : BonDbContext<BonIdentityManagementDbContext>
    , IBonIdentityManagementDbContext
{
    public BonIdentityManagementDbContext(DbContextOptions<BonIdentityManagementDbContext> options) :
        base(options)
    {
    }

    public DbSet<BonIdentityUser> Users { get; set; }
    public DbSet<BonIdentityRole> Roles { get; set; }
    public DbSet<BonIdentityPermission> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureBonIdentityManagementByConvention();
        base.OnModelCreating(modelBuilder);
    }
}