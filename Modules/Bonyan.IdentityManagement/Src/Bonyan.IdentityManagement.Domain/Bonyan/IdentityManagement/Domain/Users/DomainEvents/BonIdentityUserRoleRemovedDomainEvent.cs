using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Events;

namespace Bonyan.IdentityManagement.Domain.Users.DomainEvents;

public class BonIdentityUserRoleRemovedDomainEvent : BonDomainEventBase
{
    public BonIdentityUser User { get; }
    public BonRoleId RoleId { get; }

    public BonIdentityUserRoleRemovedDomainEvent(BonIdentityUser user, BonRoleId role)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        RoleId = role ?? throw new ArgumentNullException(nameof(role));
    }
}