using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Events;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users.DomainEvents;

/// <summary>
/// Raised when a role is removed from a user. Carries user and role identity for handlers.
/// </summary>
public class BonIdentityUserRoleRemovedDomainEvent : BonDomainEventBase
{
    /// <summary>
    /// Id of the user from whom the role was removed.
    /// </summary>
    public BonUserId UserId { get; }

    /// <summary>
    /// Id of the role that was removed.
    /// </summary>
    public BonRoleId RoleId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BonIdentityUserRoleRemovedDomainEvent"/> class.
    /// </summary>
    public BonIdentityUserRoleRemovedDomainEvent(BonUserId userId, BonRoleId roleId)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        RoleId = roleId ?? throw new ArgumentNullException(nameof(roleId));
    }
}
