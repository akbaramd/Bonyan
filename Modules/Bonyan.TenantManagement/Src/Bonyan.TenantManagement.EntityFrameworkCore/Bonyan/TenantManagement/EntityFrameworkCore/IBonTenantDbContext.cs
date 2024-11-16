using Bonyan.TenantManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.TenantManagement.EntityFrameworkCore;

public interface IBonTenantDbContext
{
  public DbSet<BonTenant> Tenants { get; set; }
}
