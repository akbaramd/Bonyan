namespace Bonyan.MultiTenant;

public interface ITenantConfigurationProvider
{
  Task<TenantConfiguration?> GetAsync(bool saveResolveResult = false);
}
