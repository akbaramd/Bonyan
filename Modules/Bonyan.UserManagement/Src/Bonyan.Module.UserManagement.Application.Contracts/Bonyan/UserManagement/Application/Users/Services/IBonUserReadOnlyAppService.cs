using Bonyan.Layer.Application.Services;
using Bonyan.UserManagement.Application.Users.Dto;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.UserManagement.Application.Users.Services;

public interface IBonUserReadOnlyAppService : IBonReadonlyAppService<BonUserId, BonUserFilterAndPaginateDto, BonUserDto>
{
}