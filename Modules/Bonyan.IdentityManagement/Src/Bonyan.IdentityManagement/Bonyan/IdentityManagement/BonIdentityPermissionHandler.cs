using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Roles.Repostories;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace Bonyan.IdentityManagement;

internal class BonIdentityPermissionHandler : AuthorizationHandler<BonPermissionRequirement>
{
    private readonly IBonIdentityRoleRepository _roleRepository;
    private readonly IBonIdentityRolePermissionRepository _rolePermissionRepository;

    public BonIdentityPermissionHandler(IBonIdentityRoleRepository roleRepository, IBonIdentityRolePermissionRepository rolePermissionRepository)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BonPermissionRequirement requirement)
    {
        var userRoleClaims = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value);
        
        // For each role, check if it has the required permission
        foreach (var role in userRoleClaims)
        {
            var rolePermissions = 
                await _rolePermissionRepository.FindAsync(x => x.RoleId == BonRoleId.NewId(role));

            // Check if role has permission
            if (rolePermissions.Any(rp => rp.PermissionId == requirement.Permission.Id))
            {
                context.Succeed(requirement);
                return;
            }
        }

        context.Fail();
    }
}