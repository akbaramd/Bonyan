using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.MultiTenant;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.TenantManagement.Domain;


public class BonyanTenantManagementDomainModule : Modularity.Abstractions.Module
{
  public BonyanTenantManagementDomainModule()
  {
    DependOn([
      typeof(BonyanMultiTenantModule),
      typeof(BonyanLayerDomainModule),
    ]);
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    context.Services.AddTransient<ITenantStore, TenantStore>();
    context.Services.AddTransient<ITenantManager, TenantManager>();
    return base.OnConfigureAsync(context);
  }
}
