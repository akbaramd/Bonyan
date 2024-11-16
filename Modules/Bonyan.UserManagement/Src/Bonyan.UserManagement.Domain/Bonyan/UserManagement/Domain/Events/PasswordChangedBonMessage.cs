using Bonyan.Layer.Domain.DomainEvent.Abstractions;

namespace Bonyan.UserManagement.Domain.Events;

/// <summary>
/// Represents an event triggered when a user's password is changed.
/// </summary>
public record PasswordChangedDomainEvent(BonUser User) : IBonDomainEvent;