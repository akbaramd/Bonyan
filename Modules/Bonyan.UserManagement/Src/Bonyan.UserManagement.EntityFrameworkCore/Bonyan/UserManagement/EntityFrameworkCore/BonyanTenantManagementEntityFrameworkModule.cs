using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.UserManagement.Domain.Users;

namespace Bonyan.UserManagement.EntityFrameworkCore;


public class BonUserManagementEntityFrameworkModule : Modularity.Abstractions.BonModule 
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
