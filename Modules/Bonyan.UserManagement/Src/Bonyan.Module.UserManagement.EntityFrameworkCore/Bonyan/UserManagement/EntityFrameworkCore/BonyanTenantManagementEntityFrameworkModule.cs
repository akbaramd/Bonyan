using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.UserManagement.Domain.Users;

namespace Bonyan.UserManagement.EntityFrameworkCore;


public class BonUserManagementEntityFrameworkModule<TUser> : Modularity.Abstractions.BonModule 
{

  public BonUserManagementEntityFrameworkModule()
  {
    DependOn([
      typeof(BonEntityFrameworkModule),
    ]);
  }
  public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
  {

    
    return base.OnConfigureAsync(context);
  }
}
