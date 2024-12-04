using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;

namespace Bonyan.IdentityManagement.Application.Roles;

public interface
    IBonIdentityRoleAppService : IBonCrudAppService<BonRoleId, BonFilterAndPaginateDto,BonIdentityRoleCreateDto,BonIdentityRoleUpdateDto,
    BonIdentityRoleDto>
{
}