using Bonyan.Layer.Domain.Events;

namespace Bonyan.TenantManagement.Domain.Events;

public class TenantUpdatedBonDomainEvent : BonDomainEventBase
{
    public TenantUpdatedBonDomainEvent(BonTenantId bonTenantId, string key)
    {
        BonTenantId = bonTenantId;
        Key = key;
    }

    public BonTenantId BonTenantId { get; set; }
    public string Key { get; set; }
}