using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Bonyan.UserManagement.Application.Users.Dtos;
using Bonyan.UserManagement.Domain.Users.Entities;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.UserManagement.Application.Users.Services;

public class BonUserCrudService<TUser> : CrudAppService<TUser,BonUserId,BonUserDto> , IBonUserCrudService where TUser : class, IBonUser
{
    public BonUserCrudService(IBonRepository<TUser, BonUserId> repository) : base(repository)
    {
    }
}