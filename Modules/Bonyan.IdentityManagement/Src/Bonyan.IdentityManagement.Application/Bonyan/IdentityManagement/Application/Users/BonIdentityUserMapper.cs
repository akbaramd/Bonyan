using AutoMapper;
using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Domain.Enumerations;
using Bonyan.UserManagement.Domain.Users.Enumerations;

namespace Bonyan.IdentityManagement.Application.Users;

public class BonIdentityUserMapper<TUser> : Profile where TUser : IBonIdentityUser
{
    public BonIdentityUserMapper()
    {
        CreateMap<TUser, BonIdentityUserDto>();
        CreateMap<BonIdentityUserRegistererDto, TUser>().ReverseMap();
        
        CreateMap<BonIdentityUserCreateDto, TUser>()
            .ForMember(x=>x.Status,b=>b.MapFrom(n=>BonEnumeration.FromId<UserStatus>(n.Status)));
        CreateMap<BonIdentityUserUpdateDto, TUser>();

    }
}