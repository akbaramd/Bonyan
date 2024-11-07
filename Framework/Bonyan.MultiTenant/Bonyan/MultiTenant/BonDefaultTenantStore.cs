using Microsoft.Extensions.Options;

namespace Bonyan.MultiTenant;

public class BonDefaultTenantStore : IBonTenantStore
{
  private readonly BonDefaultTenantStoreOptions _options;

  public BonDefaultTenantStore(IOptionsMonitor<BonDefaultTenantStoreOptions> options)
  {
    _options = options.CurrentValue;
  }

  public Task<BonTenantConfiguration?> FindAsync(string name)
  {
    return Task.FromResult(Find(name));
  }

  public Task<BonTenantConfiguration?> FindAsync(Guid id)
  {
    return Task.FromResult(Find(id));
  }

  public BonTenantConfiguration? Find(string name)
  {
    return _options.Tenants?.FirstOrDefault(t => t.Name == name);
  }

  public BonTenantConfiguration? Find(Guid id)
  {
    return _options.Tenants?.FirstOrDefault(t => t.Id == id);
  }
}
