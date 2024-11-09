using Bonyan.EntityFrameworkCore;
using Bonyan.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.UserManagement.EntityFrameworkCore;

public class BonUserManagementDbContext<TUser> : BonDbContext<BonUserManagementDbContext<TUser>>,IBonUserManagementDbContext<TUser> where TUser : BonUser
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ConfigureBonUserManagementByConvention<TUser>();
  }

  public DbSet<TUser> Users { get; set; }

  public BonUserManagementDbContext(DbContextOptions<BonUserManagementDbContext<TUser>> options) : base(options)
  {
    
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.EnableSensitiveDataLogging();
    base.OnConfiguring(optionsBuilder);
  }
}

public class BonUserManagementDbContext : BonDbContext<BonUserManagementDbContext>,IBonUserManagementDbContext
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ConfigureBonUserManagementByConvention<BonUser>();
  }

  public DbSet<BonUser> Users { get; set; }

  public BonUserManagementDbContext(DbContextOptions<BonUserManagementDbContext> options) : base(options)
  {
    
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.EnableSensitiveDataLogging();
    base.OnConfiguring(optionsBuilder);
  }
}
