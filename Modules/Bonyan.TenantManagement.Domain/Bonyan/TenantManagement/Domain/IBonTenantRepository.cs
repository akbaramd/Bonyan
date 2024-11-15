using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.TenantManagement.Domain;

public interface IBonTenantRepository : IBonRepository<BonTenant, BonTenantId>
{
    public Task<BonTenant?> FindByKeyAsync(string key, CancellationToken? cancellationToken = null);
    
}