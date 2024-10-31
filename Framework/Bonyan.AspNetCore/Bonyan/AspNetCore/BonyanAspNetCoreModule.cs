using Bonyan.AspNetCore.Security;
using Bonyan.ExceptionHandling;
using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.UnitOfWork;
using Microsoft;

namespace Bonyan.AspNetCore;

/// <summary>
/// Configures services and middleware specific to the Bonyan ASP.NET Core module.
/// </summary>

public class BonyanAspNetCoreModule : WebModule
{

    public BonyanAspNetCoreModule()
    {
        DependOn<BonyanLayerDomainModule>();
    }
    /// <summary>
    /// Configures exception handling settings before the module configuration phase.
    /// </summary>
    /// <param name="context">The configuration context for services.</param>
    public override Task OnPreConfigureAsync(ServiceConfigurationContext context)
    {
        ConfigureExceptionHandling(context);
        return base.OnPreConfigureAsync(context);
    }

    /// <summary>
    /// Configures services required by the ASP.NET Core module.
    /// </summary>
    /// <param name="context">The configuration context for services.</param>
    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {
        ConfigureMiddlewares(context);
        ConfigureCoreServices(context);
        return base.OnConfigureAsync(context);
    }

    /// <summary>
    /// Applies application-wide configurations, such as enabling authorization middleware.
    /// </summary>
    /// <param name="context">The application context used during application initialization.</param>
    public override Task OnApplicationAsync(ApplicationContext context)
    {
        context.Application.UseAuthorization();
        return base.OnApplicationAsync(context);
    }

    /// <summary>
    /// Configures exception handling options for the application.
    /// </summary>
    /// <param name="context">The configuration context for services.</param>
    private void ConfigureExceptionHandling(ServiceConfigurationContext context)
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
    private void ConfigureMiddlewares(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<BonyanClaimsMapMiddleware>();
        context.Services.AddTransient<BonyanUnitOfWorkMiddleware>();
    }

    /// <summary>
    /// Configures core services required for the ASP.NET Core application.
    /// </summary>
    /// <param name="context">The configuration context for services.</param>
    private void ConfigureCoreServices(ServiceConfigurationContext context)
    {
        context.Services.AddAuthorization();
        context.Services.AddHttpContextAccessor();
        
        // Ensures IApplicationBuilder is accessible throughout the application's service configuration lifecycle
        context.Services.AddObjectAccessor<IApplicationBuilder>();
    }
}
