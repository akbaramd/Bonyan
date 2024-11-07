using Bonyan.MultiTenant;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.MultiTenant;

public class MultiTenancyMiddleware : IMiddleware
{
    public ILogger<MultiTenancyMiddleware> Logger { get; set; }

    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;
    private readonly IBonCurrentTenant _bonCurrentTenant;
    private readonly BonAspNetCoreMultiTenancyOptions _options;
    private readonly ITenantResolveResultAccessor _tenantResolveResultAccessor;

    public MultiTenancyMiddleware(
        ITenantConfigurationProvider tenantConfigurationProvider,
        IBonCurrentTenant bonCurrentTenant,
        IOptions<BonAspNetCoreMultiTenancyOptions> options,
        ITenantResolveResultAccessor tenantResolveResultAccessor)
    {
        Logger = NullLogger<MultiTenancyMiddleware>.Instance;

        _tenantConfigurationProvider = tenantConfigurationProvider;
        _bonCurrentTenant = bonCurrentTenant;
        _tenantResolveResultAccessor = tenantResolveResultAccessor;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        BonTenantConfiguration? tenant = null;
        try
        {
            tenant = await _tenantConfigurationProvider.GetAsync(saveResolveResult: true);
        }
        catch (Exception )
        {
         

        }

        if (tenant?.Id != _bonCurrentTenant.Id)
        {
            using (_bonCurrentTenant.Change(tenant?.Id, tenant?.Name))
            {
                if (_tenantResolveResultAccessor.Result != null &&
                    _tenantResolveResultAccessor.Result.AppliedResolvers.Contains(QueryStringTenantResolveContributor.ContributorName))
                {
                    BonMultiTenancyCookieHelper.SetTenantCookie(context, _bonCurrentTenant.Id, _options.TenantKey);
                }

                await next(context);
            }
        }
        else
        {
            await next(context);
        }
    }

   
}
