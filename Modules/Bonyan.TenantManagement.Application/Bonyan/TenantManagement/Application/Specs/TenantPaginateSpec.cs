using Bonyan.Layer.Domain.Abstractions;
using Bonyan.Layer.Domain.Specifications;
using Bonyan.TenantManagement.Application.Dto;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.Application.Specs;

public class TenantPaginateSpec : BonPaginatedSpecification<BonTenant>
{
  private readonly BonTenantFilterDto _filterDto;

  public TenantPaginateSpec(BonTenantFilterDto filterDto) : base(filterDto.Skip,filterDto.Take)
  {
    _filterDto = filterDto;
  }

  public override void Handle(IBonSpecificationContext<BonTenant> context)
  {
    context.AddCriteria(x=>_filterDto.Search == null || x.Key.Contains(_filterDto.Search));
  }
}
