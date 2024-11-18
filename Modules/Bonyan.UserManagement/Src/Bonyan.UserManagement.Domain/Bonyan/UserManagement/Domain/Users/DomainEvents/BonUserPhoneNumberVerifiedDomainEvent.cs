using Bonyan.Layer.Domain.DomainEvent.Abstractions;

namespace Bonyan.UserManagement.Domain.Users.DomainEvents;

/// <summary>
/// Represents an event triggered when a user's phone number is verified.
/// </summary>
public record BonUserPhoneNumberVerifiedDomainEvent(BonUser User) : IBonDomainEvent;