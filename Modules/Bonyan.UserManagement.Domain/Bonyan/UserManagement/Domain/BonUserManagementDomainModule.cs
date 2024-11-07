using Bonyan.Layer.Domain;
using Bonyan.Modularity;

namespace Bonyan.UserManagement.Domain;


public class BonUserManagementDomainModule<TUser> : Modularity.Abstractions.BonModule where TUser : BonUser
{

  public BonUserManagementDomainModule()
  {
    DependOn([
      typeof(BonLayerDomainModule),
    ]);
  }
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    
    return base.OnConfigureAsync(context);
  }
}
