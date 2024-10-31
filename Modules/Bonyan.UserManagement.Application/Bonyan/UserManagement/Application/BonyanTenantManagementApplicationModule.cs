using Bonyan.Layer.Application;
using Bonyan.Modularity;
using Bonyan.UserManagement.Domain;

namespace Bonyan.UserManagement.Application;


public class BonyanUserManagementApplicationModule<TUser> : Modularity.Abstractions.Module where TUser : BonyanUser
{
  public BonyanUserManagementApplicationModule()
  {
    DependOn<BonyanLayerApplicationModule>();
    DependOn<BonyanUserManagementDomainModule<TUser>>();
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    return base.OnConfigureAsync(context);
  }
}
