﻿using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Events;

namespace Bonyan.IdentityManagement.Domain.Users.DomainEvents;

public class BonIdentityUserRoleAddedDomainEvent : BonDomainEventBase
{
    public readonly object User;
    public readonly BonRoleId Role;

    public BonIdentityUserRoleAddedDomainEvent(object user, BonRoleId role)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        Role = role ?? throw new ArgumentNullException(nameof(role));
    }
}