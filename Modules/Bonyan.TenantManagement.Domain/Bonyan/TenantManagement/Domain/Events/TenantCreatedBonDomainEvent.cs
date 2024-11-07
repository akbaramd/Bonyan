using Bonyan.Layer.Domain.Events;

namespace Bonyan.TenantManagement.Domain.Events;

public class TenantCreatedBonDomainEvent : BonDomainEventBase
{
    public TenantCreatedBonDomainEvent(BonTenantId bonTenantId, string key)
    {
        BonTenantId = bonTenantId;
        Key = key;
    }

    public BonTenantId BonTenantId { get; set; }
    public string Key { get; set; }
}