using Bonyan.MultiTenant;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.MultiTenant;

[Obsolete("This may make some features of ASP NET Core unavailable, Will be removed in future versions.")]
public class FormTenantResolveContributor : HttpTenantResolveContributorBase
{
    public const string ContributorName = "Form";

    public override string Name => ContributorName;

    protected override async Task<string?> GetTenantIdOrNameFromHttpContextOrNullAsync(ITenantResolveContext context, HttpContext httpContext)
    {
        if (!httpContext.Request.HasFormContentType)
        {
            return null;
        }

        var form = await httpContext.Request.ReadFormAsync();
        var tenantKey = context.ServiceProvider.GetRequiredService<IOptions<BonAspNetCoreMultiTenancyOptions>>().Value.TenantKey;
        return form[tenantKey];
    }
}
