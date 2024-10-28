using Bonyan.DomainDrivenDesign.Domain;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.EntityFramework;

public class TenantRepository : EfCoreRepository<Tenant,TenantId,BonyanTenantDbContext>,ITenantRepository
{
  public TenantRepository(BonyanTenantDbContext dbContext) : base(dbContext)
  {
  }
}
