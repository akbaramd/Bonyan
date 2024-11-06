using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.MultiTenant;


public class BonyanMultiTenantModule : Module
{
  public BonyanMultiTenantModule()
  {
    DependOn<BonyanSecurityModule>();
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    context.Services.AddTransient<ITenantResolver,TenantResolver>();
    context.Services.AddTransient<ITenantStore,DefaultTenantStore>();
    context.Services.AddTransient<ITenantConfigurationProvider,TenantConfigurationProvider>();
    context.Services.AddSingleton<ICurrentTenantAccessor>(AsyncLocalCurrentTenantAccessor.Instance);
    context.Services.AddSingleton<ICurrentTenant, CurrentTenant>();
    return base.OnConfigureAsync(context);
  }

  public override Task OnPostConfigureAsync(ServiceConfigurationContext context)
  {
    return base.OnPostConfigureAsync(context);
  }
  


}