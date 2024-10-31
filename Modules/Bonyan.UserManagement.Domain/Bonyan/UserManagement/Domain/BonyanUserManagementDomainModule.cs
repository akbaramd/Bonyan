using Bonyan.Layer.Domain;
using Bonyan.Modularity;

namespace Bonyan.UserManagement.Domain;


public class BonyanUserManagementDomainModule<TUser> : Modularity.Abstractions.Module where TUser : BonyanUser
{

  public BonyanUserManagementDomainModule()
  {
    DependOn([
      typeof(BonyanLayerDomainModule),
    ]);
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    
    return base.OnConfigureAsync(context);
  }
}
