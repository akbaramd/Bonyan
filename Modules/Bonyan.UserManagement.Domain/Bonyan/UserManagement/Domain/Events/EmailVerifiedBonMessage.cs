﻿using Bonyan.Layer.Domain.DomainEvent.Abstractions;

namespace Bonyan.UserManagement.Domain.Events;

/// <summary>
/// Represents an event triggered when a user's email is verified.
/// </summary>
public record EmailVerifiedDomainEvent(BonUser User) : IBonDomainEvent;