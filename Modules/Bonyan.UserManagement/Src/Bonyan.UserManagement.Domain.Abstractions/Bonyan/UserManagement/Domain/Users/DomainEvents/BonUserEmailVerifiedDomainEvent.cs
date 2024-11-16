using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.UserManagement.Domain.Users.Entities;

namespace Bonyan.UserManagement.Domain.Users.DomainEvents;

/// <summary>
/// Represents an event triggered when a user's email is verified.
/// </summary>
public record BonUserEmailVerifiedDomainEvent(IBonUser User) : IBonDomainEvent;