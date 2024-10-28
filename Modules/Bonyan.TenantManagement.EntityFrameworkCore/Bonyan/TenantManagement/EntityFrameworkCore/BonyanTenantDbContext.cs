using Bonyan.AspNetCore.Persistence.EntityFrameworkCore;
using Bonyan.TenantManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.TenantManagement.EntityFramework;

public class BonyanTenantDbContext : BonyanDbContext<BonyanTenantDbContext>,IBonyanTenantDbContext
{
  public BonyanTenantDbContext(DbContextOptions<BonyanTenantDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
  {
  }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ConfigureTenantManagementByConvention();
  }
  public DbSet<Tenant> Tenants { get; set; }
}
