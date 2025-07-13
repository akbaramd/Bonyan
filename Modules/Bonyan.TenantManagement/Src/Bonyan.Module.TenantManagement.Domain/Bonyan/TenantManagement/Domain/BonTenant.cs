using Bonyan.Layer.Domain.Aggregate;
using Bonyan.TenantManagement.Domain.Events;

namespace Bonyan.TenantManagement.Domain;

public class BonTenant : BonFullAggregateRoot<BonTenantId>
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
