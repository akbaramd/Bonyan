using Bonyan.Layer.Domain;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.EntityFrameworkCore;

public class BonTenantRepository : EfCoreBonRepository<BonTenant, BonTenantId, BonTenantDbContext>, IBonTenantRepository
{
    public BonTenantRepository(BonTenantDbContext dbContext, IServiceProvider serviceProvider) : base(dbContext)
    {
    }

    public Task<BonTenant?> FindByKeyAsync(string key, CancellationToken? cancellationToken = null)
    {
        return FindOneAsync(x => x.Key == key);
    }
    
  
}