using Bonyan.Layer.Application.Dto;
using Bonyan.TenantManagement.Domain;

namespace Bonyan.TenantManagement.Application.Dto;

public class TenantDto : FullAuditableAggregateRootDto<TenantId>
{
  public string Key { get; set; } = default!;
}
public class TenantCreateDto 
{
  public string Key { get; set; } = default!;
}
public class TenantUpdateDto 
{
  public string Key { get; set; } = default!;
}
public class TenantFilterDto : FilterAndPaginateDto 
{
}