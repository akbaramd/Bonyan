using Bonyan.Layer.Domain.Aggregates;
using Bonyan.TenantManagement.Domain.Events;

namespace Bonyan.TenantManagement.Domain;

public class BonTenant : BonFullAuditableAggregateRoot<BonTenantId>
{
  public string Key { get; set; }

  public BonTenant(string key)
  {
    Id = new BonTenantId();
    Key = key;
    AddDomainEvent(new TenantUpdatedBonDomainEvent(Id,Key));
  }

  public void SetKey(string key)
  {
    Key = key;
    AddDomainEvent(new TenantUpdatedBonDomainEvent(Id,Key));
  }
}
