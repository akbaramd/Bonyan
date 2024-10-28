using Bonyan.Layer.Domain.Abstractions;

namespace Bonyan.TenantManagement.Domain;

public interface ITenantRepository : IRepository<Tenant, TenantId>
{
    public Task<Tenant?> FindByKeyAsync(string key, CancellationToken? cancellationToken = null);
    public Task<Tenant?> FindByNameAsync(string name, CancellationToken? cancellationToken = null);
    
}