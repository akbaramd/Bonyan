using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;
using Bonyan.MultiTenant;

namespace Bonyan.UserManagement.Domain;

[DependOn([
  typeof(BonyanMultiTenantModule),
  typeof(BonyanLayerDomainModule),
])]
public class BonyanUserManagementDomainModule : Modularity.Abstractions.Module
{
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    return base.OnConfigureAsync(context);
  }
}
