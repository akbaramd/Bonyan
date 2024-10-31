using Bonyan.EntityFrameworkCore;
using Bonyan.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.UserManagement.EntityFrameworkCore;

public class BonUserManagementDbContext<TUser> : BonyanDbContext<BonUserManagementDbContext<TUser>>,IBonUserManagementDbContext<TUser> where TUser : BonyanUser
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ConfigureUserManagementByConvention<TUser>();
    base.OnModelCreating(modelBuilder);
  }

  public DbSet<TUser> Users { get; set; }

  public BonUserManagementDbContext(DbContextOptions<BonUserManagementDbContext<TUser>> options) : base(options)
  {
  }
}