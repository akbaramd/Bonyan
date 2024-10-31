using Bonyan.EntityFrameworkCore;
using Bonyan.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public class BonUserManagementManagementDbContext<TUser> : BonyanDbContext<BonUserManagementManagementDbContext<TUser>>,IBonUserManagementDbContext<TUser> where TUser : BonyanUser
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ConfigureUserManagementByConvention<TUser>();
    base.OnModelCreating(modelBuilder);
  }

  public DbSet<TUser> Users { get; set; }

  public BonUserManagementManagementDbContext(DbContextOptions<BonUserManagementManagementDbContext<TUser>> options) : base(options)
  {
  }
}
