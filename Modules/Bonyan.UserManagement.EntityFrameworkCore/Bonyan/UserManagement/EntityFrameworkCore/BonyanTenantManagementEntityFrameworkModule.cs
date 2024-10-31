using Bonyan.AspNetCore.MultiTenant;
using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;

namespace Bonyan.UserManagement.EntityFrameworkCore;


public class BonyanUserManagementEntityFrameworkModule : Modularity.Abstractions.Module
{

  public BonyanUserManagementEntityFrameworkModule()
  {
    DependOn([
      typeof(BonyanEntityFrameworkModule),
      typeof(BonyanAspNetCoreMultiTenantModule)
    ]);
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    return base.OnConfigureAsync(context);
  }
}
