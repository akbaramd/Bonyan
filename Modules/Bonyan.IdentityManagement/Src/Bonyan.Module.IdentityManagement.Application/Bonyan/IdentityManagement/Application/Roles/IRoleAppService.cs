using Bonyan.IdentityManagement.Application.Roles.Dtos;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Application.Services;

namespace Bonyan.IdentityManagement.Application.Roles;

/// <summary>
/// Application service for role CRUD. Returns <see cref="ServiceResult"/> and <see cref="ServiceResult{T}"/>.
/// </summary>
public interface IRoleAppService : IBonCrudAppService<BonRoleId, RoleFilterDto, RoleCreateDto, RoleUpdateDto, RoleDto>
{
}
