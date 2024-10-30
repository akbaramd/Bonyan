using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.MultiTenant;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.TenantManagement.Domain;

[DependOn([
  typeof(BonyanMultiTenantModule),
  typeof(BonyanLayerDomainModule),
])]
public class BonyanTenantManagementDomainModule : Modularity.Abstractions.Module
{
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    context.Services.AddTransient<ITenantStore, TenantStore>();
    context.Services.AddTransient<ITenantManager, TenantManager>();
    return base.OnConfigureAsync(context);
  }
}
