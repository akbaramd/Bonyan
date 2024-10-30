using Bonyan.AspNetCore.MultiTenant;
using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;

namespace Bonyan.TenantManagement.EntityFrameworkCore;

[DependOn(typeof(BonyanEntityFrameworkModule),
  typeof(BonyanAspNetCoreMultiTenantModule))]
public class BonyanUserManagementEntityFrameworkModule : Modularity.Abstractions.Module
{
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    return base.OnConfigureAsync(context);
  }
}
