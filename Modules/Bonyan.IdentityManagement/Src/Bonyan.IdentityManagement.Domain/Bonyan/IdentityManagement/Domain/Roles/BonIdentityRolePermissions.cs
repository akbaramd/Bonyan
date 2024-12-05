using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Domain.Entities;

namespace Bonyan.IdentityManagement.Domain.Roles;

public class BonIdentityRolePermissions : BonEntity
{
    public BonRoleId RoleId { get; private set; }
    public BonPermissionId PermissionId { get; private set; }
    public BonIdentityPermission Permission { get; private set; }
    public BonIdentityRole Role { get; private set; }

    protected BonIdentityRolePermissions() { } // For ORM use

    public BonIdentityRolePermissions(BonRoleId role, BonPermissionId permission)
    {
        RoleId = role;
        PermissionId = permission;
    }

    public override object GetKey()
    {
        return new { RoleId, PermissionId };
    }
}