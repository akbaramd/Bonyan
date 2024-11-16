using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Entities;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bonyan.IdentityManagement.Domain.Users;

public class BonIdentityUserRoles : BonEntity
{
    // Parameterless constructor for EF Core
    protected BonIdentityUserRoles(BonUserId userId, BonRoleId roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public BonUserId UserId { get; set; }
    public BonRoleId RoleId { get; set; }

    public override object GetKey()
    {
        return new { UserId, RoleId };
    }
}