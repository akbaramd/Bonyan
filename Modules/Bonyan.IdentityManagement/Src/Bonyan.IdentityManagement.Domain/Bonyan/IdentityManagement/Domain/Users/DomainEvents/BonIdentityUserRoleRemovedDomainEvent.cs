﻿using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.Layer.Domain.Events;

namespace Bonyan.IdentityManagement.Domain.Users.DomainEvents;

public class BonIdentityUserRoleRemovedDomainEvent : BonDomainEventBase
{
    public BonIdentityUser User { get; }
    public BonIdentityRole Role { get; }

    public BonIdentityUserRoleRemovedDomainEvent(BonIdentityUser user, BonIdentityRole role)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        Role = role ?? throw new ArgumentNullException(nameof(role));
    }
}