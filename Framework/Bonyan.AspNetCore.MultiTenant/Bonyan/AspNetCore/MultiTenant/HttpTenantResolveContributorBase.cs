using Bonyan.MultiTenant;
using JetBrains.Annotations;

namespace Bonyan.AspNetCore.MultiTenant;

public abstract class HttpTenantResolveContributorBase : TenantResolveContributorBase
{
    public override async Task ResolveAsync(ITenantResolveContext context)
    {
        var httpContext = context.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
        if (httpContext == null)
        {
            return;
        }

        try
        {
            await ResolveFromHttpContextAsync(context, httpContext);
        }
        catch (Exception e)
        {
            context.ServiceProvider
                .GetRequiredService<ILogger<HttpTenantResolveContributorBase>>()
                .LogWarning(e.ToString());
        }
    }

    protected virtual async Task ResolveFromHttpContextAsync(ITenantResolveContext context, HttpContext httpContext)
    {
        var tenantIdOrName = await GetTenantIdOrNameFromHttpContextOrNullAsync(context, httpContext);
        if (!tenantIdOrName.IsNullOrEmpty())
        {
            context.TenantIdOrName = tenantIdOrName;
        }
    }

    protected abstract Task<string?> GetTenantIdOrNameFromHttpContextOrNullAsync([NotNull] ITenantResolveContext context, [NotNull] HttpContext httpContext);
}
