using Bonyan.TenantManagement.Domain.Bonyan.TenantManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builder;

namespace Bonyan.TenantManagement.EntityFrameworkCore.Bonyan.TenantManagement.EntityFrameworkCore;

public static class EntityTypeBuilderExtensions
{
  public static ModelBuilder ConfigureTenantManagementByConvention(this ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Tenant>().ConfigureByConvention();
    return modelBuilder;
  }
}
