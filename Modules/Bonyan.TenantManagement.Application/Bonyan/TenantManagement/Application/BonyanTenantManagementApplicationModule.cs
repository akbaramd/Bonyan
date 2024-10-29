using Bonyan.Layer.Application;
using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;

namespace Bonyan.TenantManagement.Application;

[DependOn(typeof(BonyanLayerApplicationModule))]
public class BonyanTenantManagementApplicationModule : Modularity.Abstractions.Module
{
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    return base.OnConfigureAsync(context);
  }
}
