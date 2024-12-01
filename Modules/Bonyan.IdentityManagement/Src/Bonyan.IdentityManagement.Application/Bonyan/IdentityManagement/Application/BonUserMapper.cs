using AutoMapper;
using Bonyan.IdentityManagement.Application.Dto;

namespace Bonyan.IdentityManagement.Application;

public class BonUserMapper<TUser> : Profile
{
    public BonUserMapper()
    {
        CreateMap<BonIdentityUserDto, TUser>().ReverseMap();
        CreateMap<BonIdentityUserRegistererDto, TUser>().ReverseMap();
    }
}