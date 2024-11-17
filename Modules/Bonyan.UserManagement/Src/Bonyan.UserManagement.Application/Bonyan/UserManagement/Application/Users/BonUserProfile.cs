using AutoMapper;
using Bonyan.UserManagement.Application.Users.Dtos;
using Bonyan.UserManagement.Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Bonyan.UserManagement.Application.Users;

public class BonUserProfile : Profile
{
    public BonUserProfile()
    {
        CreateMap<BonUser, BonUserDto>().ReverseMap();
    }
}