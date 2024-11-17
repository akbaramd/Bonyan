using Bonyan.EntityFrameworkCore;
using Bonyan.TenantManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.TenantManagement.EntityFrameworkCore;

public class TenantBonDbContext : BonDbContext<TenantBonDbContext>,IBonTenantDbContext
{
  public TenantBonDbContext(DbContextOptions<TenantBonDbContext> options) : base(options)
  {
    Console.Write(ServiceProvider);
  }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ConfigureTenantManagementByConvention();
  }
  public DbSet<BonTenant> Tenants { get; set; }
}
