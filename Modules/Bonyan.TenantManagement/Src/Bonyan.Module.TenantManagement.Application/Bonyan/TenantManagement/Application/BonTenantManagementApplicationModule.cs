using Bonyan.AutoMapper;
using Bonyan.Layer.Application;
using Bonyan.Modularity;
using Bonyan.TenantManagement.Application.Profiles;
using Bonyan.TenantManagement.Application.Services;
using Bonyan.TenantManagement.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.TenantManagement.Application;


public class BonTenantManagementApplicationModule : Modularity.Abstractions.BonModule
{
  public BonTenantManagementApplicationModule()
  {
    DependOn([
      typeof(BonLayerApplicationModule),
      typeof(BonTenantManagementDomainModule)
    ]);
  }
  public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
  {
    context.Services.AddTransient<IBonTenantBonApplicationService, BonTenantBonApplicationService>();

    context.Services.Configure<BonAutoMapperOptions>(c =>
    {
      c.AddProfile<BonTenantProfile>(true);
    });
    
    return base.OnConfigureAsync(context);
  }
}
