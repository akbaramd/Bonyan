using Bonyan.Layer.Domain.Events;

namespace Bonyan.UserManagement.Domain.Events;

/// <summary>
/// Represents an event triggered when a user is created.
/// </summary>
public record UserCreatedEvent(BonyanUser User) : IDomainEvent;