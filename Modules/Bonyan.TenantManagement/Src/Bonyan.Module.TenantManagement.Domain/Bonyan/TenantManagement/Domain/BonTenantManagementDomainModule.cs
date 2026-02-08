using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.MultiTenant;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.TenantManagement.Domain;


public class BonTenantManagementDomainModule : Modularity.Abstractions.BonModule
{
  public BonTenantManagementDomainModule()
  {
    DependOn([
      typeof(BonMultiTenantModule),
      typeof(BonLayerDomainModule),
    ]);
  }
  public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
  {
    context.Services.AddTransient<IBonTenantStore, BonTenantStore>();
    context.Services.AddTransient<IBonTenantManager, BonTenantManager>();
    return base.OnConfigureAsync(context);
  }
}
