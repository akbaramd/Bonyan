using Bonyan.Layer.Application;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.Domain;

namespace Bonyan.UserManagement.Application;


public class BonUserManagementApplicationModule<TUser> : BonModule where TUser : BonUser
{
  public BonUserManagementApplicationModule()
  {
    DependOn<BonLayerApplicationModule>();
    DependOn<BonUserManagementDomainModule<TUser>>();
  }
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    return base.OnConfigureAsync(context);
  }
}
