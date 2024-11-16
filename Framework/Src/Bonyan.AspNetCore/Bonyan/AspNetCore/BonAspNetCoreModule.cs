using Bonyan.AspNetCore.Security;
using Bonyan.ExceptionHandling;
using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.Security.Claims;
using Bonyan.UnitOfWork;
using Microsoft;

namespace Bonyan.AspNetCore;

/// <summary>
/// Configures services and middleware specific to the Bonyan ASP.NET Core module.
/// </summary>

public class BonAspNetCoreModule : BonWebModule
{

    public BonAspNetCoreModule()
    {
        DependOn<BonLayerDomainModule>();
    }
    /// <summary>
    /// Configures exception handling settings before the module configuration phase.
    /// </summary>
    /// <param name="context">The configuration context for services.</param>
    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        ConfigureExceptionHandling(context);
        return base.OnPreConfigureAsync(context);
    }

    /// <summary>
    /// Configures services required by the ASP.NET Core module.
    /// </summary>
    /// <param name="context">The configuration context for services.</param>
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddLogging();
        ConfigureMiddlewares(context);
        ConfigureCoreServices(context);
        return base.OnConfigureAsync(context);
    }

    /// <summary>
    /// Applies application-wide configurations, such as enabling authorization middleware.
    /// </summary>
    /// <param name="webApplicationContext">The application context used during application initialization.</param>
    public override Task OnApplicationAsync(BonWebApplicationContext webApplicationContext)
    {
        webApplicationContext.Application.UseHttpsRedirection();
        return base.OnApplicationAsync(webApplicationContext);
    }

    /// <summary>
    /// Configures exception handling options for the application.
    /// </summary>
    /// <param name="context">The configuration context for services.</param>
    private void ConfigureExceptionHandling(BonConfigurationContext context)
    {
        context.ConfigureOptions<ExceptionHandlingOptions>(options => 
        {
            options.ApiExceptionMiddlewareEnabled = false;
        });
    }

    /// <summary>
    /// Configures middleware dependencies for the application.
    /// </summary>
    /// <param name="context">The configuration context for services.</param>
    private void ConfigureMiddlewares(BonConfigurationContext context)
    {
        context.Services.AddTransient<BonyanClaimsMapMiddleware>();
        context.Services.AddTransient<BonyanUnitOfWorkMiddleware>();
    }

    /// <summary>
    /// Configures core services required for the ASP.NET Core application.
    /// </summary>
    /// <param name="context">The configuration context for services.</param>
    private void ConfigureCoreServices(BonConfigurationContext context)
    {
        context.Services.AddAuthorization();
        context.Services.AddHttpContextAccessor();
        context.Services.AddTransient<IBonCurrentPrincipalAccessor, HttpContextBonCurrentPrincipalAccessor>();
        // Ensures IApplicationBuilder is accessible throughout the application's service configuration lifecycle
        context.Services.AddObjectAccessor<IApplicationBuilder>();
    }
}
