using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;

namespace Bonyan.IdentityManagement.Application.Permissions;

public interface
    IBonIdentityPermissionAppService : IBonReadonlyAppService<BonPermissionId, BonFilterAndPaginateDto,
    BonIdentityPermissionDto>
{
}