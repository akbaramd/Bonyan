using Bonyan.Layer.Domain.Events;

namespace Bonyan.TenantManagement.Domain.Events;

public class TenantCreatedDomainEvent : DomainEventBase
{
    public TenantCreatedDomainEvent(TenantId tenantId, string key)
    {
        TenantId = tenantId;
        Key = key;
    }

    public TenantId TenantId { get; set; }
    public string Key { get; set; }
}