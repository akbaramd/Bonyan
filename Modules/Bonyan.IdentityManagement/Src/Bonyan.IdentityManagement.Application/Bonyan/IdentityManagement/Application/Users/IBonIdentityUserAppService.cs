using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Application.Roles;

public interface
    IBonIdentityUserAppService : IBonCrudAppService<BonUserId, BonFilterAndPaginateDto,BonIdentityUserCreateDto,BonIdentityUserUpdateDto,
    BonIdentityUserDto>
{
}