using Bonyan.Layer.Domain.DomainEvent.Abstractions;

namespace Bonyan.UserManagement.Domain.Events;

/// <summary>
/// Represents an event triggered when a user's profile is updated.
/// </summary>
public record UserProfileUpdatedDomainEvent(BonUser User) : IBonDomainEvent;