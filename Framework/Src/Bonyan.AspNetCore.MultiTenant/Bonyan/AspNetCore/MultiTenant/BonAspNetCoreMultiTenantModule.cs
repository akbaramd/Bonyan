using Bonyan.Modularity;
using Bonyan.MultiTenant;

namespace Bonyan.AspNetCore.MultiTenant;


public class BonAspNetCoreMultiTenantModule : BonWebModule
{

  public BonAspNetCoreMultiTenantModule()
  {
    DependOn([
      typeof(BonMultiTenantModule),
      typeof(BonAspNetCoreModule)
    ]);
  }
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    context.Services.AddTransient<MultiTenancyMiddleware>();
    context.Services.AddSingleton<ITenantResolveResultAccessor, HttpContextTenantResolveResultAccessor>();
    context.ConfigureOptions<BonTenantResolveOptions>(options =>
    {
      options.TenantResolvers.Add(new QueryStringTenantResolveContributor());
      options.TenantResolvers.Add(new RouteTenantResolveContributor());
      options.TenantResolvers.Add(new HeaderTenantResolveContributor());
      options.TenantResolvers.Add(new CookieTenantResolveContributor());
    });
    return base.OnConfigureAsync(context);
  }

  public override Task OnPreApplicationAsync(BonWebApplicationContext webApplicationContext)
  {
    return base.OnPreApplicationAsync(webApplicationContext);
  }
}
