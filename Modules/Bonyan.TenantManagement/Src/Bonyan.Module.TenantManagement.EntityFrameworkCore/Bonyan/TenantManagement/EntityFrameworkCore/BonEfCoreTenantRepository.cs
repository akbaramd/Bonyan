using Bonyan.Layer.Domain;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.EntityFrameworkCore;

public class BonEfCoreTenantRepository : EfCoreBonRepository<BonTenant, BonTenantId, IBonTenantDbContext>, IBonTenantRepository
{
 

    public Task<BonTenant?> FindByKeyAsync(string key, CancellationToken? cancellationToken = null)
    {
        return FindOneAsync(x => x.Key == key);
    }
    
  
}