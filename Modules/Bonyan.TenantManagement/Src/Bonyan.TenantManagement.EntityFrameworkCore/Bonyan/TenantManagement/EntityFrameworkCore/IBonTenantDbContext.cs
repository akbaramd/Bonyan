using Bonyan.EntityFrameworkCore;
using Bonyan.TenantManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.TenantManagement.EntityFrameworkCore;

public interface IBonTenantDbContext : IEfDbContext
{
  public DbSet<BonTenant> Tenants { get; set; }
}
