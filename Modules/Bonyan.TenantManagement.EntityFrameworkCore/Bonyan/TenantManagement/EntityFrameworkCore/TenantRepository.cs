using Bonyan.DomainDrivenDesign.Domain;
using Bonyan.TenantManagement.Domain.Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.EntityFrameworkCore.Bonyan.TenantManagement.EntityFrameworkCore;

public class TenantRepository : EfCoreRepository<Tenant,TenantId,BonyanTenantDbContext>,ITenantRepository
{
  public TenantRepository(BonyanTenantDbContext dbContext , IServiceProvider  serviceProvider) : base(dbContext,serviceProvider)
  {
  }
}
