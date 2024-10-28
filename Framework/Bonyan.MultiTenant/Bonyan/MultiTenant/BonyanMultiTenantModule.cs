using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;
using Bonyan.Security;
using Microsoft.Extensions.DependencyInjection;
using Module = Bonyan.Modularity.Abstractions.Module;

namespace Bonyan.MultiTenant;

[DependOn(typeof(BonyanUserModule))]
public class BonyanMultiTenantModule : Module
{
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