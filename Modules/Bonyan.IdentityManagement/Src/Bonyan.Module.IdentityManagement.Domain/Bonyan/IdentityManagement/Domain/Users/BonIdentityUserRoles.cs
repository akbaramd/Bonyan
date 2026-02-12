using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Entities;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users;

/// <summary>
/// User–Role link entity (part of User aggregate). References Role only by BonRoleId (other aggregate).
/// </summary>
public class BonIdentityUserRoles : BonEntity
{
    public BonUserId UserId { get; private set; } = null!;
    public BonRoleId RoleId { get; private set; } = null!;

    protected BonIdentityUserRoles() { }

    public BonIdentityUserRoles(BonUserId userId, BonRoleId roleId)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        RoleId = roleId ?? throw new ArgumentNullException(nameof(roleId));
    }

    public override object GetKey() => new { UserId, RoleId };
}
