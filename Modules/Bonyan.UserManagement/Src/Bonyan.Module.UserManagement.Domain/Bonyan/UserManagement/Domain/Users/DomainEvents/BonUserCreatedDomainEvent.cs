using Bonyan.Layer.Domain.DomainEvent.Abstractions;

namespace Bonyan.UserManagement.Domain.Users.DomainEvents;

/// <summary>
/// Represents an event triggered when a user is created.
/// </summary>
public record BonUserCreatedDomainEvent(BonUser User) : IBonDomainEvent;