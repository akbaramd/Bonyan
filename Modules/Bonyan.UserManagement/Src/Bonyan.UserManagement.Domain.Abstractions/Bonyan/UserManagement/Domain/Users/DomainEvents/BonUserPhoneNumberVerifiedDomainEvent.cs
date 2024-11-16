using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.UserManagement.Domain.Users.Entities;

namespace Bonyan.UserManagement.Domain.Users.DomainEvents;

/// <summary>
/// Represents an event triggered when a user's phone number is verified.
/// </summary>
public record BonUserPhoneNumberVerifiedDomainEvent(IBonUser User) : IBonDomainEvent;