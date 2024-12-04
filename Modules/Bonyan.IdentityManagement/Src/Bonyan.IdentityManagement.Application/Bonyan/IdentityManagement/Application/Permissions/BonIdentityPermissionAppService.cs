using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Permissions.Repositories;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.IdentityManagement.Application.Permissions;

public class BonIdentityPermissionAppService :
    BonReadonlyAppService<BonIdentityPermission, BonPermissionId, BonFilterAndPaginateDto, BonIdentityPermissionDto>,
    IBonIdentityPermissionAppService
{
   
}