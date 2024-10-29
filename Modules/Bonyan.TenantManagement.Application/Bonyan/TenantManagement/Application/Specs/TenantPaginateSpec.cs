using Bonyan.Layer.Domain.Specifications;
using Bonyan.TenantManagement.Application.Dto;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.Application.Specs;

public class TenantPaginateSpec : PaginatedSpecification<Tenant>
{
  private readonly TenantFilterDto _filterDto;

  public TenantPaginateSpec(TenantFilterDto filterDto) : base(filterDto.Skip,filterDto.Take)
  {
    _filterDto = filterDto;
  }

  public override void Handle(ISpecificationContext<Tenant> context)
  {
    context.AddCriteria(x=>_filterDto.Search != null && x.Key.Contains(_filterDto.Search));
  }
}
