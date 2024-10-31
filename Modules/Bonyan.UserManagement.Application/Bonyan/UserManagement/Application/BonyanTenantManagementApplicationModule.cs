using Bonyan.Layer.Application;
using Bonyan.Modularity;
using Bonyan.UserManagement.Domain;

namespace Bonyan.UserManagement.Application;


public class BonyanUserManagementApplicationModule : Modularity.Abstractions.Module
{
  public BonyanUserManagementApplicationModule()
  {
    DependOn<BonyanLayerApplicationModule>();
    DependOn<BonyanUserManagementDomainModule>();
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    return base.OnConfigureAsync(context);
  }
}
