
namespace Bonyan.MultiTenant;

public interface ITenantResolveContext 
{
  public IServiceProvider  ServiceProvider { get;  }
    string? TenantIdOrName { get; set; }

    bool Handled { get; set; }
}
