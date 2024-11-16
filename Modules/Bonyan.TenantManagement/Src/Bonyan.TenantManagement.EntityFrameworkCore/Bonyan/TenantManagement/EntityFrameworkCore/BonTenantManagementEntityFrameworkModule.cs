using Bonyan.AspNetCore.MultiTenant;
using Bonyan.DependencyInjection;
using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.TenantManagement.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.TenantManagement.EntityFrameworkCore;


public class BonTenantManagementEntityFrameworkModule : Modularity.Abstractions.BonModule
{
  public BonTenantManagementEntityFrameworkModule()
  {
    DependOn(typeof(BonEntityFrameworkModule),
      typeof(BonAspNetCoreMultiTenantModule));
  }
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    context.Services.AddTransient<IBonTenantRepository, BonTenantRepository>();
    
    return base.OnConfigureAsync(context);
  }
}
