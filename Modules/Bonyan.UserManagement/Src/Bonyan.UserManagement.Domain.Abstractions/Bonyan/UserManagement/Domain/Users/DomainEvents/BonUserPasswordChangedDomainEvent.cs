using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.UserManagement.Domain.Users.Entities;

namespace Bonyan.UserManagement.Domain.Users.DomainEvents;

/// <summary>
/// Represents an event triggered when a user's password is changed.
/// </summary>
public record BonUserPasswordChangedDomainEvent(IBonUser User) : IBonDomainEvent;