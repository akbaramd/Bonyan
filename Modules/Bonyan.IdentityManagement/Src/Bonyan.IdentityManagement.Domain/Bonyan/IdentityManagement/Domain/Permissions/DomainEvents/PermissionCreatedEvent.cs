using Bonyan.Layer.Domain.Events;

namespace Bonyan.IdentityManagement.Domain.Permissions.DomainEvents;

public class PermissionCreatedEvent : BonDomainEventBase
{
    public string Name { get; set; }
}