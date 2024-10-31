using Bonyan.Layer.Domain.Aggregates;
using Bonyan.TenantManagement.Domain.Events;

namespace Bonyan.TenantManagement.Domain;

public class Tenant : FullAuditableAggregateRoot<TenantId>
{
  public string Key { get; set; }

  public Tenant(string key)
  {
    Id = new TenantId();
    Key = key;
    AddDomainEvent(new TenantUpdatedDomainEvent(Id,Key));
  }

  public void SetKey(string key)
  {
    Key = key;
    AddDomainEvent(new TenantUpdatedDomainEvent(Id,Key));
  }
}
