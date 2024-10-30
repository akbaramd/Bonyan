using Bonyan.AspNetCore.MultiTenant;
using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;

namespace Bonyan.UserManagement.EntityFrameworkCore;

[DependOn(typeof(BonyanEntityFrameworkModule),
  typeof(BonyanAspNetCoreMultiTenantModule))]
public class BonyanUserManagementEntityFrameworkModule : Modularity.Abstractions.Module
{
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    return base.OnConfigureAsync(context);
  }
}
