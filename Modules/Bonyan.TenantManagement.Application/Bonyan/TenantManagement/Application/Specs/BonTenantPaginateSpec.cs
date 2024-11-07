using Bonyan.Layer.Domain.Specifications;
using Bonyan.TenantManagement.Application.Dto;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.Application.Specs;

public class BonTenantPaginateSpec : PaginatedSpecification<BonTenant>
{
  private readonly BonTenantFilterDto _filterDto;

  public BonTenantPaginateSpec(BonTenantFilterDto filterDto) : base(filterDto.Skip,filterDto.Take)
  {
    _filterDto = filterDto;
  }

  public override void Handle(ISpecificationContext<BonTenant> context)
  {
    context.AddCriteria(x=>_filterDto.Search == null || x.Key.Contains(_filterDto.Search));
  }
}
