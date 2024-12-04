using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Roles.Repositories;
using Microsoft.AspNetCore.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IBonIdentityRoleRepository _roleRepository;
    private readonly IBonIdentityRolePermissionRepository _rolePermissionRepository;

    public PermissionHandler(IBonIdentityRoleRepository roleRepository, IBonIdentityRolePermissionRepository rolePermissionRepository)
    {
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userRoleClaims = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value);
        
        // For each role, check if it has the required permission
        foreach (var role in userRoleClaims)
        {
            var rolePermissions = 
                await _rolePermissionRepository.FindAsync(x => x.RoleId.Value == role);

            // Check if role has permission
            if (rolePermissions.Any(rp => rp.PermissionId.Value == requirement.Permission))
            {
                context.Succeed(requirement);
                return;
            }
        }

        context.Fail();
    }
}