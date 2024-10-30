using Bonyan.MultiTenant;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.MultiTenant;

public class MultiTenancyMiddleware : IMiddleware
{
    public ILogger<MultiTenancyMiddleware> Logger { get; set; }

    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;
    private readonly ICurrentTenant _currentTenant;
    private readonly BonyanAspNetCoreMultiTenancyOptions _options;
    private readonly ITenantResolveResultAccessor _tenantResolveResultAccessor;

    public MultiTenancyMiddleware(
        ITenantConfigurationProvider tenantConfigurationProvider,
        ICurrentTenant currentTenant,
        IOptions<BonyanAspNetCoreMultiTenancyOptions> options,
        ITenantResolveResultAccessor tenantResolveResultAccessor)
    {
        Logger = NullLogger<MultiTenancyMiddleware>.Instance;

        _tenantConfigurationProvider = tenantConfigurationProvider;
        _currentTenant = currentTenant;
        _tenantResolveResultAccessor = tenantResolveResultAccessor;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        TenantConfiguration? tenant = null;
        try
        {
            tenant = await _tenantConfigurationProvider.GetAsync(saveResolveResult: true);
        }
        catch (Exception )
        {
         

        }

        if (tenant?.Id != _currentTenant.Id)
        {
            using (_currentTenant.Change(tenant?.Id, tenant?.Name))
            {
                if (_tenantResolveResultAccessor.Result != null &&
                    _tenantResolveResultAccessor.Result.AppliedResolvers.Contains(QueryStringTenantResolveContributor.ContributorName))
                {
                    BonyanMultiTenancyCookieHelper.SetTenantCookie(context, _currentTenant.Id, _options.TenantKey);
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
