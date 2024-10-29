using Bonyan.EntityFrameworkCore;
using Bonyan.IoC.Autofac;
using Bonyan.TenantManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.TenantManagement.EntityFrameworkCore;

public class BonyanTenantDbContext : BonyanDbContext<BonyanTenantDbContext>,IBonyanTenantDbContext
{
  public BonyanTenantDbContext(DbContextOptions<BonyanTenantDbContext> options) : base(options)
  {
    Console.Write(ServiceProvider);
  }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ConfigureTenantManagementByConvention();
  }
  public DbSet<Tenant> Tenants { get; set; }
}
