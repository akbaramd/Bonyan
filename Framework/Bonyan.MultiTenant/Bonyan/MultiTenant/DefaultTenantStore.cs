using Microsoft.Extensions.Options;

namespace Bonyan.MultiTenant;

public class DefaultTenantStore : ITenantStore
{
  private readonly AbpDefaultTenantStoreOptions _options;

  public DefaultTenantStore(IOptionsMonitor<AbpDefaultTenantStoreOptions> options)
  {
    _options = options.CurrentValue;
  }

  public Task<TenantConfiguration?> FindAsync(string name)
  {
    return Task.FromResult(Find(name));
  }

  public Task<TenantConfiguration?> FindAsync(Guid id)
  {
    return Task.FromResult(Find(id));
  }

  public TenantConfiguration? Find(string name)
  {
    return _options.Tenants?.FirstOrDefault(t => t.Name == name);
  }

  public TenantConfiguration? Find(Guid id)
  {
    return _options.Tenants?.FirstOrDefault(t => t.Id == id);
  }
}
