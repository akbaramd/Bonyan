using Bonyan.Layer.Domain.DomainEvent.Abstractions;

namespace Bonyan.UserManagement.Domain.Users.DomainEvents;

/// <summary>
/// Represents an event triggered when a user's email is verified.
/// </summary>
public record BonUserEmailVerifiedDomainEvent(BonUser User) : IBonDomainEvent;