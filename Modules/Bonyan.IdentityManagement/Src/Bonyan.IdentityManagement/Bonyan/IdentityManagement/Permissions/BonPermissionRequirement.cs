using Bonyan.IdentityManagement.Domain.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace Bonyan.IdentityManagement.Permissions;

public class BonPermissionRequirement : IAuthorizationRequirement
{
    public BonIdentityPermission Permission { get; }

    public BonPermissionRequirement(BonIdentityPermission permission)
    {
        Permission = permission;
    }
}