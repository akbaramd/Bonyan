using AutoMapper;
using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Domain.Permissions.Repositories;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.DomainServices;

public class RolePermissionsResolver : IValueResolver<BonIdentityRole, BonIdentityRoleDto, List<BonIdentityPermissionDto>>
{
    private readonly IBonIdentityRoleManager _identityRoleManager;

    public RolePermissionsResolver(IBonIdentityRoleManager identityRoleManager)
    {
        _identityRoleManager = identityRoleManager;
    }

    public List<BonIdentityPermissionDto> Resolve(BonIdentityRole source, BonIdentityRoleDto destination, List<BonIdentityPermissionDto> destMember, ResolutionContext context)
    {
        // Use repository to fetch permissions by RoleId
        return _identityRoleManager.FindPermissionByRoleIdAsync(source.Id.Value).GetAwaiter().GetResult().Value
            .Select(c=>new BonIdentityPermissionDto()
            {
                Id = c.Id.Value,
                Title = c.Title
            }).ToList(); // Ensure asynchronous handling
    }
}