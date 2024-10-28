using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;
using Bonyan.MultiTenant;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.TenantManagement.Domain;

[DependOn(typeof(BonyanMultiTenantModule))]
public class BonyanTenantManagementDomainModule : Modularity.Abstractions.Module
{
  public override Task OnConfigureAsync(ModularityContext context)
  {
    context.Services.AddTransient<ITenantStore, TenantStore>();
    return base.OnConfigureAsync(context);
  }
}
