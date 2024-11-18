using Bonyan.Layer.Domain.DomainEvent.Abstractions;

namespace Bonyan.UserManagement.Domain.Users.DomainEvents;

/// <summary>
/// Represents an event triggered when a user's profile is updated.
/// </summary>
public record BonUserProfileUpdatedDomainEvent(BonUser User) : IBonDomainEvent;