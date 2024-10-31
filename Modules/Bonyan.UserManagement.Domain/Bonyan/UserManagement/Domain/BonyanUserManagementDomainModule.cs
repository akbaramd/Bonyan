using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.MultiTenant;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.UserManagement.Domain;


public class BonyanUserManagementDomainModule : Modularity.Abstractions.Module
{

  public BonyanUserManagementDomainModule()
  {
    DependOn([
      typeof(BonyanMultiTenantModule),
      typeof(BonyanLayerDomainModule),
    ]);
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    return base.OnConfigureAsync(context);
  }
}
