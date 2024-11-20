using AutoMapper;
using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.UserManagement.Application.Users.Dto;

namespace Bonyan.IdentityManagement.Application;

public class BonUserMapper<TUser> : Profile
{
    public BonUserMapper()
    {
        CreateMap<BonIdentityUserDto, TUser>().ReverseMap();
        CreateMap<BonIdentityUserRegistererDto, TUser>().ReverseMap();
    }
}