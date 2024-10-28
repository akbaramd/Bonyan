using Bonyan.MultiTenant;

namespace Bonyan.TenantManagement.Domain;

public class TenantStore : ITenantStore
{
    protected ITenantRepository TenantRepository { get; }
    protected ICurrentTenant CurrentTenant { get; }

    public TenantStore(
        ITenantRepository tenantRepository,
        ICurrentTenant currentTenant)
    {
        TenantRepository = tenantRepository;
        CurrentTenant = currentTenant;
    }

    public virtual async Task<TenantConfiguration?> FindAsync(string name)
    {
      var tenant = await TenantRepository.FindOneAsync(x => x.Key == name);
      if (tenant != null)
      {
        return new TenantConfiguration(tenant.Id.Value, tenant.Key);
      }

      return null;
    }

    public virtual async Task<TenantConfiguration?> FindAsync(Guid id)
    {
      var tenant = await TenantRepository.FindOneAsync(x => x.Id.Value == id);
      if (tenant != null)
      {
        return new TenantConfiguration(tenant.Id.Value, tenant.Key);
      }

      return null;
    }



   

}
