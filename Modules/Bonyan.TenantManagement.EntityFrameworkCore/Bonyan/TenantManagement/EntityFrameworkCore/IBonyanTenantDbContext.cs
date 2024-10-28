using Bonyan.TenantManagement.Domain.Bonyan.TenantManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.TenantManagement.EntityFrameworkCore.Bonyan.TenantManagement.EntityFrameworkCore;

public interface IBonyanTenantDbContext
{
  public DbSet<Tenant> Tenants { get; set; }
}
