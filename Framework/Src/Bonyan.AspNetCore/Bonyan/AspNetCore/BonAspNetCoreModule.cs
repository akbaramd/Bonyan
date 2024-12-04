using Bonyan.AspNetCore.Security;
using Bonyan.AspNetCore.Tracing;
using Bonyan.ExceptionHandling;
using Bonyan.Layer.Domain;
using Bonyan.Modularity;
using Bonyan.Reflection;
using Bonyan.Security.Claims;
using Bonyan.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
        context.Services.AddAntiforgery();
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
        
        
        Configure<BonEndpointRouterOptions>(options =>
        {
            options.EndpointConfigureActions.Add(endpointContext =>
            {
                endpointContext.Endpoints.MapGet("/bonyan/modules",
                    ([FromServices] IBonModuleContainer container) =>
                    {
                        return container.Modules.Select(module => new
                        {
                            Name = module.ModuleType.Name,
                            Namespace = module.ModuleType.Namespace,
                            Assembly = module.ModuleType.Assembly.FullName,
                            Dependencies = module.Dependencies.Select(dep => new
                            {
                                Name = dep.ModuleType.Name,
                                Namespace = dep.ModuleType.Namespace,
                                Assembly = dep.ModuleType.Assembly.FullName
                            }),
                            IsLoadedAsPlugIn = module.IsLoaded,
                            AllAssemblies = module.AllAssemblies.Select(assembly => assembly.FullName).ToList(),
                            IsActive = module.Instance != null,
                            Timestamp = DateTime.UtcNow // Optionally include a timestamp for when this data was fetched
                        });
                    });
            });
        });
        
        return base.OnConfigureAsync(context);
    }

    /// <summary>
    /// Applies application-wide configurations, such as enabling authorization middleware.
    /// </summary>
    /// <param name="webApplicationContext">The application context used during application initialization.</param>
    public override Task OnApplicationAsync(BonWebApplicationContext webApplicationContext)
    {
        webApplicationContext.Application.UseHttpsRedirection();
        webApplicationContext.Application.UseAntiforgery();
        webApplicationContext.Application.UseRouting();
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
        context.Services.AddTransient<BonCorrelationIdMiddleware>();
    }

    /// <summary>
    /// Configures core services required for the ASP.NET Core application.
    /// </summary>
    /// <param name="context">The configuration context for services.</param>
    private void ConfigureCoreServices(BonConfigurationContext context)
    {
      
        context.Services.AddHttpContextAccessor();
        context.Services.AddTransient<IBonCurrentPrincipalAccessor, HttpContextBonCurrentPrincipalAccessor>();
    }

    public override Task OnPreApplicationAsync(BonWebApplicationContext context)
    {
        context.Application.UseStaticFiles();
        return base.OnPreApplicationAsync(context);
    }

    
    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        var options = context.Application.Services
            .GetRequiredService<IOptions<BonEndpointRouterOptions>>()
            .Value;
        
        if (!options.EndpointConfigureActions.Any())
        {
            return base.OnPostApplicationAsync(context);
        }

        context.Application.UseUnitOfWork();
        context.Application.UseEndpoints(endpoints =>
        {
            using var scope = context.Application.Services.CreateScope();
            if (options.EndpointConfigureActions.Count == 0) return;

            var endpointRouteBuilderContext = new EndpointRouteBuilderContext(endpoints, scope.ServiceProvider);

            foreach (var configureAction in options.EndpointConfigureActions)
            {
                configureAction(endpointRouteBuilderContext);
            }
            
        });
        
        return base.OnPostApplicationAsync(context);
    }
}