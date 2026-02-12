using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Events;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users.DomainEvents;

/// <summary>
/// Raised when a role is assigned to a user. Carries user and role identity for handlers.
/// </summary>
public class BonIdentityUserRoleAddedDomainEvent : BonDomainEventBase
{
    /// <summary>
    /// Id of the user that was assigned the role.
    /// </summary>
    public BonUserId UserId { get; }

    /// <summary>
    /// Id of the role that was assigned.
    /// </summary>
    public BonRoleId RoleId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BonIdentityUserRoleAddedDomainEvent"/> class.
    /// </summary>
    public BonIdentityUserRoleAddedDomainEvent(BonUserId userId, BonRoleId roleId)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        RoleId = roleId ?? throw new ArgumentNullException(nameof(roleId));
    }
}
