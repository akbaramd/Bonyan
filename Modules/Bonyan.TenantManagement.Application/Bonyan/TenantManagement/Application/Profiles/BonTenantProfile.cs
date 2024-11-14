using AutoMapper;
using Bonyan.Layer.Domain.Abstractions.Results;
using Bonyan.TenantManagement.Application.Dto;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.Application.Profiles;

public class BonTenantProfile : Profile
{
  public BonTenantProfile()
  {
    CreateMap<BonTenant, BonTenantDto>().ReverseMap();
    CreateMap<BonPaginatedResult<BonTenant>, BonPaginatedResult<BonTenantDto>>().ReverseMap();
    
  }
}
