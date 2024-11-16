using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.UserManagement.Domain.Users.Entities;

namespace Bonyan.UserManagement.Domain.Users.DomainEvents;

/// <summary>
/// Represents an event triggered when a user's profile is updated.
/// </summary>
public record BonUserProfileUpdatedDomainEvent(IBonUser User) : IBonDomainEvent;