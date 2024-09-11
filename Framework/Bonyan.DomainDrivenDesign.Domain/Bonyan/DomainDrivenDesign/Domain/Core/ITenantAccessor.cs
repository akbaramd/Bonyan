using Bonyan.DomainDrivenDesign.Domain.Abstractions;

namespace Bonyan.DomainDrivenDesign.Domain.Core;

public class TenantAccessor : ITenantAccessor
{
  public List<string>? CurrentTenant { get; set; }
}
