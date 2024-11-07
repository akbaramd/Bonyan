using Bonyan.Modularity;
using Bonyan.UserManagement.Domain;

namespace Bonyan.IdentityManagement.Domain;


public class BonIdentityManagementDomainModule<TUser,TRole> : Modularity.Abstractions.BonModule where TUser : BonUser where TRole : BonRole
{
  public BonIdentityManagementDomainModule()
  {
    DependOn([
      typeof(BonUserManagementDomainModule<TUser>),
    ]);
  }
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    return base.OnConfigureAsync(context);
  }
}
