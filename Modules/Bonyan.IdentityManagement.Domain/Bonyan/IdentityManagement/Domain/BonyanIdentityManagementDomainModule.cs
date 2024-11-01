using Bonyan.Modularity;
using Bonyan.UserManagement.Domain;

namespace Bonyan.IdentityManagement.Domain;


public class BonyanIdentityManagementDomainModule<TUser,TRole> : Modularity.Abstractions.Module where TUser : BonyanUser where TRole : BonRole
{
  public BonyanIdentityManagementDomainModule()
  {
    DependOn([
      typeof(BonyanUserManagementDomainModule<TUser>),
    ]);
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    return base.OnConfigureAsync(context);
  }
}
