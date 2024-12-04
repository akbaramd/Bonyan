using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain.Events;

public class BonIdentityUserRoleAddedDomainEvent : BonDomainEventBase
{
    public BonIdentityUser User { get; }
    public BonIdentityRole Role { get; }

    public BonIdentityUserRoleAddedDomainEvent(BonIdentityUser user, BonIdentityRole role)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        Role = role ?? throw new ArgumentNullException(nameof(role));
    }
}