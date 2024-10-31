﻿using Bonyan.Layer.Domain.Events;

namespace Bonyan.UserManagement.Domain.Events;

/// <summary>
/// Represents an event triggered when a user's password is changed.
/// </summary>
public record PasswordChangedEvent(BonyanUser User) : IDomainEvent;