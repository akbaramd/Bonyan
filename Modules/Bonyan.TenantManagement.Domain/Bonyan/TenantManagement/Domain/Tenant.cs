using Bonyan.Layer.Domain.Aggregates;

namespace Bonyan.TenantManagement.Domain;

public class Tenant : FullAuditableAggregateRoot<TenantId>
{
  public string Key { get; set; }
  public string Name { get; set; }

  public Tenant(string key,string name)
  {
    Id = new TenantId();
    Key = key;
    Name = name;
  }
}
