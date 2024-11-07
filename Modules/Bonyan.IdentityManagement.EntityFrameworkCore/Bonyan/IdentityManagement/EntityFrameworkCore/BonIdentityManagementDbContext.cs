using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain;
using Bonyan.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public class BonIdentityManagementDbContext<TUser, TRole> :
    BonDbContext<BonIdentityManagementDbContext<TUser, TRole>>
    , IBonIdentityManagementDbContext<TUser, TRole> where TUser : BonUser where TRole : BonRole
{
    public BonIdentityManagementDbContext(DbContextOptions<BonIdentityManagementDbContext<TUser, TRole>> options) :
        base(options)
    {
    }

    public DbSet<TUser> Users { get; set; }
    public DbSet<TRole> Roles { get; set; }
    public DbSet<BonPermission> Permissions { get; set; }
    public DbSet<BonUserRole<TUser, TRole>> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureBonIdentityManagementByConvention<TUser, TRole>();
        base.OnModelCreating(modelBuilder);
    }
}