namespace Bonyan.MultiTenant;

public class AbpDefaultTenantStoreOptions
{
  public TenantConfiguration[] Tenants { get; set; }

  public AbpDefaultTenantStoreOptions()
  {
    Tenants = Array.Empty<TenantConfiguration>();
  }
}