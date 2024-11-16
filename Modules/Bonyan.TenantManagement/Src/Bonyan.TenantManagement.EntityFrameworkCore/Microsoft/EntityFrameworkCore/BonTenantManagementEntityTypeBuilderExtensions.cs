using Bonyan.TenantManagement.Domain;

namespace Microsoft.EntityFrameworkCore;

public static class BonTenantManagementEntityTypeBuilderExtensions
{
  public static ModelBuilder ConfigureTenantManagementByConvention(this ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<BonTenant>().ConfigureByConvention();
    modelBuilder.Entity<BonTenant>().HasIndex(x => x.Key).IsUnique();
    
    return modelBuilder;
  }
}
