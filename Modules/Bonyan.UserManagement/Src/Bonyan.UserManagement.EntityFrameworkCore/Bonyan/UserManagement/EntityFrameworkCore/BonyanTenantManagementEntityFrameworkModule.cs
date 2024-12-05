using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.UserManagement.Domain.Users;

namespace Bonyan.UserManagement.EntityFrameworkCore;


public class BonUserManagementEntityFrameworkModule<TUser> : Modularity.Abstractions.BonModule where TUser : class, IBonUser
{

  public BonUserManagementEntityFrameworkModule()
  {
    DependOn([
      typeof(BonEntityFrameworkModule),
    ]);
  }
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {

    
    return base.OnConfigureAsync(context);
  }
}
