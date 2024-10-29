using Bonyan.Layer.Domain.Aggregates;

namespace Bonyan.TenantManagement.Domain;

public class Tenant : FullAuditableAggregateRoot<TenantId>
{
  public string Key { get; set; }

  public Tenant(string key)
  {
    Id = new TenantId();
    Key = key;
  }

  public void SetKey(string key)
  {
    Key = key;
  }
}
