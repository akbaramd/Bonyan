using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain;
using Bonyan.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public class BonIdentityManagementDbContext<TUser> :
    BonDbContext<BonIdentityManagementDbContext<TUser>>
    , IBonIdentityManagementDbContext<TUser> where TUser : BonIdentityUser
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