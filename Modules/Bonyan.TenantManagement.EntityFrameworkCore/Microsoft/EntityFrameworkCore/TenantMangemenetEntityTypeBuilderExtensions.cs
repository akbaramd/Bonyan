using Bonyan.TenantManagement.Domain;

namespace Microsoft.EntityFrameworkCore;

public static class TenantMangemenetEntityTypeBuilderExtensions
{
  public static ModelBuilder ConfigureTenantManagementByConvention(this ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Tenant>().ConfigureByConvention();
    modelBuilder.Entity<Tenant>().HasIndex(x => x.Key).IsUnique();
    
    return modelBuilder;
  }
}
