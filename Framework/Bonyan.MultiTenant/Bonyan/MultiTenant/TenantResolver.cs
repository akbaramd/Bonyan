using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.MultiTenant;

public class TenantResolver : ITenantResolver
{
  private readonly IServiceProvider _serviceProvider;
  private readonly BonTenantResolveOptions _options;

  public TenantResolver(IOptions<BonTenantResolveOptions> options, IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
    _options = options.Value;
  }

  public virtual async Task<TenantResolveResult> ResolveTenantIdOrNameAsync()
  {
    var result = new TenantResolveResult();

    using (var serviceScope = _serviceProvider.CreateScope())
    {
      var context = new TenantResolveContext(serviceScope.ServiceProvider);

      foreach (var tenantResolver in _options.TenantResolvers)
      {
        await tenantResolver.ResolveAsync(context);

        result.AppliedResolvers.Add(tenantResolver.Name);

        if (context.HasResolvedTenantOrHost())
        {
          result.TenantIdOrName = context.TenantIdOrName;
          break;
        }
      }
    }

    return result;
  }
}
