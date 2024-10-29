using AutoMapper;
using Bonyan.Layer.Domain.Model;
using Bonyan.TenantManagement.Application.Dto;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.Application.Profiles;

public class TenantProfile : Profile
{
  public TenantProfile()
  {
    CreateMap<Tenant, TenantDto>().ReverseMap();
    CreateMap<PaginatedResult<Tenant>, PaginatedResult<TenantDto>>().ReverseMap();
    
  }
}
