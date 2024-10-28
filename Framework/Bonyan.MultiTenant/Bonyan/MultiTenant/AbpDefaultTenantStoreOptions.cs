namespace Bonyan.MultiTenant;

public class BonyanDefaultTenantStoreOptions
{
  public TenantConfiguration[] Tenants { get; set; }

  public BonyanDefaultTenantStoreOptions()
  {
    Tenants = Array.Empty<TenantConfiguration>();
  }
}