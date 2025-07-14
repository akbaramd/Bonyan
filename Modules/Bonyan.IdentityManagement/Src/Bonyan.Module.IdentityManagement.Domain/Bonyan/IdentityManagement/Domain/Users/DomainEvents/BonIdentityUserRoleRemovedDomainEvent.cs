using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Events;

namespace Bonyan.IdentityManagement.Domain.Users.DomainEvents;

public class BonIdentityUserRoleRemovedDomainEvent : BonDomainEventBase
{
    public readonly object User;
    public readonly BonRoleId RoleId;

    public BonIdentityUserRoleRemovedDomainEvent(object user, BonRoleId role)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        RoleId = role ?? throw new ArgumentNullException(nameof(role));
    }
}