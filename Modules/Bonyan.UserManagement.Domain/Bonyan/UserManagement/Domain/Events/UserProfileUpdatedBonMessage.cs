using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Layer.Domain.Events;

namespace Bonyan.UserManagement.Domain.Events;

/// <summary>
/// Represents an event triggered when a user's profile is updated.
/// </summary>
public record UserProfileUpdatedDomainEvent(BonUser User) : IBonDomainEvent;