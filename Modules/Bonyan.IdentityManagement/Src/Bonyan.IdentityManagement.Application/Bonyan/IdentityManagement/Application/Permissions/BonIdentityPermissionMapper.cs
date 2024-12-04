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
            .MapBusinessId(c=>c.Id,v=>v.Id)
            .ReverseMap()
            .MapBusinessIdReverse(c=>c.Id,v=>v.Id);
    }
}