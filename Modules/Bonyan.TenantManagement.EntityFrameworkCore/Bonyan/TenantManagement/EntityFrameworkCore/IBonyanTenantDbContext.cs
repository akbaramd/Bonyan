using Bonyan.TenantManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.TenantManagement.EntityFrameworkCore;

public interface IBonyanTenantDbContext
{
  public DbSet<Tenant> Tenants { get; set; }
}
