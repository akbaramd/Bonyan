using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Roles;
using Microsoft.AspNetCore.Authorization;

public class BonPermissionRequirement : IAuthorizationRequirement
{
    public BonIdentityPermission Permission { get; }

    public BonPermissionRequirement(BonIdentityPermission permission)
    {
        Permission = permission;
    }
}