﻿using AutoMapper;
using Bonyan.AutoMapper;
using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Permissions.ValueObjects;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;

namespace Bonyan.IdentityManagement.Application.Roles;

public class BonIdentityRoleMapper : Profile
{
    public BonIdentityRoleMapper()
    {
        CreateMap<BonIdentityRole, BonIdentityRoleDto>()
            .MapBusinessId(c=>c.Id,v=>v.Id)
            .ReverseMap()
            .MapBusinessIdReverse(c=>c.Id,v=>v.Id);


        CreateMap<BonIdentityRoleCreateDto, BonIdentityRole>()
            .ForMember(x => x.Id, v =>
                v.MapFrom(b => BonRoleId.NewId(b.Key)));
    }
}