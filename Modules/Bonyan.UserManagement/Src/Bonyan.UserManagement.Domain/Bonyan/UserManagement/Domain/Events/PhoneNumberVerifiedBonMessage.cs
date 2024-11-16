using Bonyan.Layer.Domain.DomainEvent.Abstractions;

namespace Bonyan.UserManagement.Domain.Events;

/// <summary>
/// Represents an event triggered when a user's phone number is verified.
/// </summary>
public record PhoneNumberVerifiedDomainEvent(BonUser User) : IBonDomainEvent;