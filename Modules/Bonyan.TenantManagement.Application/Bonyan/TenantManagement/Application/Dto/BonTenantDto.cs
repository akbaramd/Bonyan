using Bonyan.TenantManagement.Domain;
using Dto;

namespace Bonyan.TenantManagement.Application.Dto;

public class BonTenantDto : BonFullAuditableAggregateRootDto<BonTenantId>
{
  public string Key { get; set; } = default!;
}
public class BonTenantCreateDto 
{
  public string Key { get; set; } = default!;
}
public class BonTenantUpdateDto 
{
  public string Key { get; set; } = default!;
}
public class BonTenantFilterDto : FilterAndBonPaginateDto 
{
}