using Bonyan.MultiTenant;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.MultiTenant;

public class CookieTenantResolveContributor : HttpTenantResolveContributorBase
{
    public const string ContributorName = "Cookie";

    public override string Name => ContributorName;

    protected override Task<string?> GetTenantIdOrNameFromHttpContextOrNullAsync(ITenantResolveContext context, HttpContext httpContext)
    {
        return Task.FromResult(httpContext.Request.Cookies[context.ServiceProvider.GetRequiredService<IOptions<BonAspNetCoreMultiTenancyOptions>>().Value.TenantKey]);
    }
}
