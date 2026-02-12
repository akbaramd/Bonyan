using AutoMapper;
using Bonyan.IdentityManagement.Application.Roles.Dtos;
using Bonyan.IdentityManagement.Application.Users.Dtos;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;

namespace Bonyan.IdentityManagement.Application.Roles;

/// <summary>
/// AutoMapper profile for role and user-role DTOs (non-generic).
/// </summary>
public class RoleAppServiceMappingProfile : Profile
{
    public RoleAppServiceMappingProfile()
    {
        CreateMap<BonIdentityRole, RoleDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
            .ForMember(d => d.CanBeDeleted, o => o.MapFrom(s => s.CanBeDeleted));

        CreateMap<BonIdentityRole, UserRoleDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Title));
    }
}
