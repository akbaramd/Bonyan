using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;
using Bonyan.MultiTenant;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.TenantManagement.Domain;

[DependOn(typeof(BonyanMultiTenantModule))]
public class BonyanTenantManagementDomainModule : Modularity.Abstractions.Module
{
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    context.Services.AddTransient<ITenantStore, TenantStore>();
    return base.OnConfigureAsync(context);
  }
}
