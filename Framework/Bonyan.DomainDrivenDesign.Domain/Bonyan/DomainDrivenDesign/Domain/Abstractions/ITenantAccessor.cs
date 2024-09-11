namespace Bonyan.DomainDrivenDesign.Domain.Abstractions;

public interface ITenantAccessor
{
  public List<string>? CurrentTenant { get; set; } 
}
