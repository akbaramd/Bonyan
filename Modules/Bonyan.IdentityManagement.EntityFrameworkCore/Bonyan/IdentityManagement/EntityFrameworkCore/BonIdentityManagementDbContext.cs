using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public class BonIdentityManagementDbContext<TUser,TRole> : BonyanDbContext<BonIdentityManagementDbContext<TUser,TRole>>
,IBonIdentityManagementDbContext<TUser,TRole> where TUser : BonyanUser where TRole : BonRole
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ConfigureIdentityManagementByConvention<TUser>();
    base.OnModelCreating(modelBuilder);
  }

  public DbSet<TUser> Users { get; set; }
  public DbSet<TRole> Roles { get; set; }

  public BonIdentityManagementDbContext(DbContextOptions<BonIdentityManagementDbContext<TUser,TRole>> options) : base(options)
  {
  }
}
