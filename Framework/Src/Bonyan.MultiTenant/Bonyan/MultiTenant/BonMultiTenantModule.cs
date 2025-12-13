using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.MultiTenant;


public class BonMultiTenantModule : BonModule
{
  public BonMultiTenantModule()
  {
    DependOn<BonSecurityModule>();
  }
  public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
  {
    context.Services.AddTransient<ITenantResolver,TenantResolver>();
    context.Services.AddTransient<IBonTenantStore,BonDefaultTenantStore>();
    context.Services.AddTransient<ITenantConfigurationProvider,TenantConfigurationProvider>();
    context.Services.AddSingleton<ICurrentTenantAccessor>(AsyncLocalCurrentTenantAccessor.Instance);
    context.Services.AddSingleton<IBonCurrentTenant, BonCurrentTenant>();
    return base.OnConfigureAsync(context, cancellationToken);
  }

  public override ValueTask OnPostConfigureAsync(BonPostConfigurationContext context, CancellationToken cancellationToken = default)
  {
    return base.OnPostConfigureAsync(context, cancellationToken);
  }
  


}