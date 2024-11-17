using Bonyan.Layer.Application.Abstractions;
using Bonyan.Layer.Application.Dto;
using Bonyan.UserManagement.Application.Users.Dtos;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.UserManagement.Application.Users.Services;

public interface IBonUserCrudService : ICrudAppService<BonUserId,BonPaginateDto,BonUserDto>
{
    
}