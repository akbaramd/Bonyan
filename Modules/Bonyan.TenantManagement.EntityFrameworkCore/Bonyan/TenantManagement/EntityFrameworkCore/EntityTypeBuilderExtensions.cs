using Bonyan.TenantManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builder;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bonyan.TenantManagement.EntityFramework;

public static class EntityTypeBuilderExtensions
{
  public static ModelBuilder ConfigureTenantManagementByConvention(this ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Tenant>().ConfigureByConvention();
    return modelBuilder;
  }
}
