namespace Bonyan.MultiTenant;

public interface ITenantConfigurationProvider
{
  Task<BonTenantConfiguration?> GetAsync(bool saveResolveResult = false);
}
