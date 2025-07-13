using Bonyan.Layer.Domain.Specification.Abstractions;
using Bonyan.TenantManagement.Application.Dto;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.Application.Specs;

public class TenantPaginateSpec : BonPaginatedSpecification<BonTenant>
{
  private readonly TenantBonFilterAndDto _filterAndDto;

  public TenantPaginateSpec(TenantBonFilterAndDto filterAndDto) : base(filterAndDto.Skip,filterAndDto.Take)
  {
    _filterAndDto = filterAndDto;
  }

  public override void Handle(IBonSpecificationContext<BonTenant> context)
  {
    context.AddCriteria(x=>_filterAndDto.Search == null || x.Key.Contains(_filterAndDto.Search));
  }
}
