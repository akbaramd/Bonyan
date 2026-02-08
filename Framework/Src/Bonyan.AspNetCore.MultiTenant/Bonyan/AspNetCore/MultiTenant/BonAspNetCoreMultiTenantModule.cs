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
  public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
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

  public override ValueTask OnPreApplicationAsync(BonWebApplicationContext webApplicationContext,CancellationToken cancellationToken = default)
  {
    return base.OnPreApplicationAsync(webApplicationContext);
  }
}
