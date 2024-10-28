using Bonyan.MultiTenant;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.MultiTenant;

public class RouteTenantResolveContributor : HttpTenantResolveContributorBase
{
    public const string ContributorName = "Route";

    public override string Name => ContributorName;

    protected override Task<string?> GetTenantIdOrNameFromHttpContextOrNullAsync(ITenantResolveContext context, HttpContext httpContext)
    {
      var tenantKey = context.ServiceProvider.GetRequiredService<IOptions<BonyanAspNetCoreMultiTenancyOptions>>().Value.TenantKey;
        var tenantId = httpContext.GetRouteValue(tenantKey);
        return Task.FromResult(tenantId != null ? Convert.ToString(tenantId) : null);
    }
}
