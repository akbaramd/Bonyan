using Bonyan.Layer.Domain.Events;

namespace Bonyan.UserManagement.Domain.Events;

/// <summary>
/// Represents an event triggered when a user's phone number is verified.
/// </summary>
public record PhoneNumberVerifiedEvent(BonUser User) : IDomainEvent;