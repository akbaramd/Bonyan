using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.UserManagement.Domain.Users.Entities;

namespace Bonyan.UserManagement.Domain.Users;


public class BonUserManagementDomainModule<TUser> : Modularity.Abstractions.BonModule where TUser : IBonUser
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
