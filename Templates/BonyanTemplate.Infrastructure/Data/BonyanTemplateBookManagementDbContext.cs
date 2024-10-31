using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.EntityFrameworkCore;
using Bonyan.TenantManagement.Domain;
using Bonyan.TenantManagement.EntityFrameworkCore;
using Bonyan.UserManagement.EntityFrameworkCore;
using BonyanTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BonyanTemplate.Infrastructure.Data;

public class BonyanTemplateBookManagementDbContext : BonyanDbContext<BonyanTemplateBookManagementDbContext> , IBonyanTenantDbContext , IBonIdentityManagementDbContext<User,Role>
{

  public BonyanTemplateBookManagementDbContext(DbContextOptions<BonyanTemplateBookManagementDbContext> options):base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<Books>().ConfigureByConvention();
    modelBuilder.Entity<Authors>().ConfigureByConvention();
    modelBuilder.ConfigureTenantManagementByConvention();
    modelBuilder.ConfigureIdentityManagementByConvention<User>();
  }

  public DbSet<Books> Books { get; set; }
  public DbSet<Authors> Authors { get; set; }
  public DbSet<Tenant> Tenants { get; set; }
  public DbSet<User> Users { get; set; }
  public DbSet<Role> Roles { get; set; }
}
