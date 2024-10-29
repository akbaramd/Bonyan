using Bonyan.Layer.Domain;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.EntityFrameworkCore;

public class TenantRepository : EfCoreRepository<Tenant, TenantId, BonyanTenantDbContext>, ITenantRepository
{
    public TenantRepository(BonyanTenantDbContext dbContext, IServiceProvider serviceProvider) : base(dbContext)
    {
    }

    public Task<Tenant?> FindByKeyAsync(string key, CancellationToken? cancellationToken = null)
    {
        return FindOneAsync(x => x.Key == key);
    }
    
  
}