using Bonyan.Layer.Application.Dto;
using Bonyan.TenantManagement.Domain;

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
public class TenantBonFilterAndDto : BonFilterAndPaginateDto 
{
}