using AutoMapper;
using Bonyan.UserManagement.Application.Users.Dto;
using Bonyan.UserManagement.Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Bonyan.UserManagement.Application.Users;

public class BonUserProfile : Profile
{
    public BonUserProfile()
    {
        CreateMap<BonUser, BonUserDto>().ReverseMap();
        CreateMap<BonUser, BonUserCreateDto>().ReverseMap();
        CreateMap<BonUser, BonUserUpdateDto>().ReverseMap();
    }
}