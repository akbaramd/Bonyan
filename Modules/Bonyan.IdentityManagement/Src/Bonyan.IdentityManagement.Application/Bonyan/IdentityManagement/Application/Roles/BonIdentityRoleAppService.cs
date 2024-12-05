using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Application.Permissions;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Permissions.Repositories;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.DomainServices;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Application.Roles;

public class BonIdentityRoleAppService :
    BonCrudAppService<BonIdentityRole, BonRoleId, BonFilterAndPaginateDto, BonIdentityRoleDto, BonIdentityRoleDto,
        BonIdentityRoleCreateDto, BonIdentityRoleUpdateDto>,
    IBonIdentityRoleAppService
{
    public IBonIdentityRoleManager RoleManager => LazyServiceProvider.LazyGetRequiredService<IBonIdentityRoleManager>();

    public override async Task<ServiceResult<BonIdentityRoleDto>> CreateAsync(BonIdentityRoleCreateDto input)
    {

        var role = BonIdentityRole.CreateDeletable(BonRoleId.NewId(input.Key), input.Title);
        var permissionIds = input.Permissions.Select(BonPermissionId.NewId);
        await RoleManager.CreateRoleWithPermissionsAsync(role,permissionIds);
        return  ServiceResult<BonIdentityRoleDto>.Success(MapToDto(role));
    }

    public override async Task<ServiceResult<BonIdentityRoleDto>> UpdateAsync(BonRoleId id,
        BonIdentityRoleUpdateDto input)
    {
        var roleResult = await RoleManager.FindRoleByIdAsync(id.Value);
        var role = roleResult.Value;
        role.UpdateTitle(input.Title);
        var permissionIds = input.Permissions.Select(BonPermissionId.NewId);
        await RoleManager.UpdateRoleAsync(role);
        await RoleManager.AssignPermissionsToRoleAsync(role, permissionIds);

        return  ServiceResult<BonIdentityRoleDto>.Success(MapToDto(role));
    }
}