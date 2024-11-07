namespace Bonyan.MultiTenant;

public class BonDefaultTenantStoreOptions
{
  public BonTenantConfiguration[] Tenants { get; set; }

  public BonDefaultTenantStoreOptions()
  {
    Tenants = Array.Empty<BonTenantConfiguration>();
  }
}