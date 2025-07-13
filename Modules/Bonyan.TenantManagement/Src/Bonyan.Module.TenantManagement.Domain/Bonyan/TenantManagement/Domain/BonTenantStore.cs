using Bonyan.MultiTenant;

namespace Bonyan.TenantManagement.Domain;

public class BonTenantStore : IBonTenantStore
{
    protected IBonTenantRepository BonTenantRepository { get; }
    protected IBonCurrentTenant BonCurrentTenant { get; }

    public BonTenantStore(
        IBonTenantRepository bonTenantRepository,
        IBonCurrentTenant bonCurrentTenant)
    {
        BonTenantRepository = bonTenantRepository;
        BonCurrentTenant = bonCurrentTenant;
    }

    public virtual async Task<BonTenantConfiguration?> FindAsync(string name)
    {
      var tenant = await BonTenantRepository.FindOneAsync(x => x.Key == name);
      if (tenant != null)
      {
        return new BonTenantConfiguration(tenant.Id.Value, tenant.Key);
      }

      return null;
    }

    public virtual async Task<BonTenantConfiguration?> FindAsync(Guid id)
    {
      var tenant = await BonTenantRepository.FindOneAsync(x => x.Id == BonTenantId.NewId(id));
      if (tenant != null)
      {
        return new BonTenantConfiguration(tenant.Id.Value, tenant.Key);
      }

      return null;
    }



   

}
