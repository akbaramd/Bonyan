using Bonyan.Layer.Domain.Events;

namespace Bonyan.TenantManagement.Domain.Events;

public class TenantDeletedDomainEvent : DomainEventBase
{
    public TenantDeletedDomainEvent(TenantId tenantId)
    {
        TenantId = tenantId;
    }

    public TenantId TenantId { get; set; }
}