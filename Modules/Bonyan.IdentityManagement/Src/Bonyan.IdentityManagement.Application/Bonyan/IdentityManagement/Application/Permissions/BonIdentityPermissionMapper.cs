using AutoMapper;
using Bonyan.AutoMapper;
using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Domain.Permissions;

namespace Bonyan.IdentityManagement.Application.Permissions;

public class BonIdentityPermissionMapper : Profile
{
    public BonIdentityPermissionMapper()
    {
        CreateMap<BonIdentityPermission, BonIdentityPermissionDto>()
            .ReverseMap();
    }
}