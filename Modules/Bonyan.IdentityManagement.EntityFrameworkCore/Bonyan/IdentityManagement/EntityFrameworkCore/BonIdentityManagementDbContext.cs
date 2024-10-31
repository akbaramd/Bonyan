using Bonyan.EntityFrameworkCore;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public class BonIdentityManagementDbContext<TUser> : BonyanDbContext<BonIdentityManagementDbContext<TUser> >, IBonIdentityManagementDbContext<TUser> where TUser : BonyanUser
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ConfigureIdentityManagementByConvention<TUser>();
    base.OnModelCreating(modelBuilder);
  }

  public DbSet<TUser> Users { get; set; }

  public BonIdentityManagementDbContext(DbContextOptions<BonIdentityManagementDbContext<TUser>> options) : base(options)
  {
  }
}
