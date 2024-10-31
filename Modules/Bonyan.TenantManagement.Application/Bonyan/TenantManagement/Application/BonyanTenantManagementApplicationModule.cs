using Bonyan.AutoMapper;
using Bonyan.Layer.Application;
using Bonyan.Modularity;
using Bonyan.TenantManagement.Application.Profiles;
using Bonyan.TenantManagement.Application.Services;
using Bonyan.TenantManagement.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.TenantManagement.Application;


public class BonyanTenantManagementApplicationModule : Modularity.Abstractions.Module
{
  public BonyanTenantManagementApplicationModule()
  {
    DependOn([
      typeof(BonyanLayerApplicationModule),
      typeof(BonyanTenantManagementDomainModule)
    ]);
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    context.Services.AddTransient<ITenantApplicationService, TenantApplicationService>();

    context.Services.Configure<BonyanAutoMapperOptions>(c =>
    {
      c.AddProfile<TenantProfile>(true);
    });
    
    return base.OnConfigureAsync(context);
  }
}
