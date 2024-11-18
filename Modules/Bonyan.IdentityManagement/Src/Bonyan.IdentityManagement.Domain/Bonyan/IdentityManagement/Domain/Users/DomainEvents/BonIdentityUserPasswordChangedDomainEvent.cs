using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.UserManagement.Domain.Users;

namespace Bonyan.IdentityManagement.Domain.Users.DomainEvents;

/// <summary>
/// Represents an event triggered when a user's password is changed.
/// </summary>
public record BonIdentityUserPasswordChangedDomainEvent(IBonUser User) : IBonDomainEvent;