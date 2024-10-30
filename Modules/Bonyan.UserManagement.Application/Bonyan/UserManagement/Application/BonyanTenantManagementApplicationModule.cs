using Bonyan.Layer.Application;
using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;
using Bonyan.UserManagement.Domain;

namespace Bonyan.UserManagement.Application;

[DependOn([
  typeof(BonyanLayerApplicationModule),
  typeof(BonyanUserManagementDomainModule)
])]
public class BonyanUserManagementApplicationModule : Modularity.Abstractions.Module
{
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    return base.OnConfigureAsync(context);
  }
}
