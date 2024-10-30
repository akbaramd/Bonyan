using Bonyan.Modularity;
using Bonyan.MultiTenant;

namespace Bonyan.AspNetCore.MultiTenant;

[DependOn(
    typeof(BonyanMultiTenantModule),
    typeof(BonyanAspNetCoreModule)
    )]
public class BonyanAspNetCoreMultiTenantModule : WebModule
{
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    context.Services.AddTransient<MultiTenancyMiddleware>();
    context.Services.AddSingleton<ITenantResolveResultAccessor, HttpContextTenantResolveResultAccessor>();
    context.Configure<BonyanTenantResolveOptions>(options =>
    {
      options.TenantResolvers.Add(new QueryStringTenantResolveContributor());
      options.TenantResolvers.Add(new RouteTenantResolveContributor());
      options.TenantResolvers.Add(new HeaderTenantResolveContributor());
      options.TenantResolvers.Add(new CookieTenantResolveContributor());
    });
    return base.OnConfigureAsync(context);
  }

  public override Task OnPreApplicationAsync(ApplicationContext context)
  {
    context.Application
      .UseMiddleware<MultiTenancyMiddleware>();
    return base.OnPreApplicationAsync(context);
  }
}
