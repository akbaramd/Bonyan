using Bonyan.Layer.Domain.Events;

namespace Bonyan.TenantManagement.Domain.Events;

public class TenantDeletedBonDomainEvent : BonDomainEventBase
{
    public TenantDeletedBonDomainEvent(BonTenantId bonTenantId)
    {
        BonTenantId = bonTenantId;
    }

    public BonTenantId BonTenantId { get; set; }
}