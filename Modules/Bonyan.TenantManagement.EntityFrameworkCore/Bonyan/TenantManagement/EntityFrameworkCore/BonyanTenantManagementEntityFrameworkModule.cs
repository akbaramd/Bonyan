using Bonyan.AspNetCore.MultiTenant;
using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.TenantManagement.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.TenantManagement.EntityFrameworkCore;


public class BonyanTenantManagementEntityFrameworkModule : Modularity.Abstractions.Module
{
  public BonyanTenantManagementEntityFrameworkModule()
  {
    DependOn(typeof(BonyanEntityFrameworkModule),
      typeof(BonyanAspNetCoreMultiTenantModule));
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    context.Services.AddTransient<ITenantRepository, TenantRepository>();
    context.AddBonyanDbContext<BonyanTenantDbContext>(c =>
    {
      c.AddDefaultRepositories();
    });
    
    return base.OnConfigureAsync(context);
  }
}
