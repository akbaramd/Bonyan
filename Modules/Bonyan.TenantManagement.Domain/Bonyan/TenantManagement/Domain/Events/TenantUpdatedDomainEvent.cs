using Bonyan.Layer.Domain.Events;

namespace Bonyan.TenantManagement.Domain.Events;

public class TenantUpdatedDomainEvent : DomainEventBase
{
    public TenantUpdatedDomainEvent(TenantId tenantId, string key)
    {
        TenantId = tenantId;
        Key = key;
    }

    public TenantId TenantId { get; set; }
    public string Key { get; set; }
}