using Bonyan.AspNetCore.Security;
using Bonyan.AspNetCore.Tracing;
using Bonyan.ExceptionHandling;
using Bonyan.Modularity;
using Bonyan.Reflection;
using Bonyan.Security.Claims;
using Bonyan.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore;

/// <summary>
/// Configures services and middleware specific to the Bonyan ASP.NET Core module.
/// Presentation/Infrastructure Layer - should NOT depend on Domain Layer directly.
/// If Domain services are needed, they should be accessed through Application Layer.
/// </summary>
public class BonAspNetCoreModule : BonWebModule
{
    public BonAspNetCoreModule()
    {
        // No dependencies - Presentation Layer should be independent
        // Domain Layer should be loaded separately if needed (via Application Layer)
    }

    /// <summary>
    /// Configures exception handling settings before the module configuration phase.
    /// </summary>
    /// <param name="context">The configuration context for services.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public override ValueTask OnPreConfigureAsync(BonPreConfigurationContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        context.Services.AddAntiforgery();
        // Exception handling configuration moved to OnConfigureAsync
        return base.OnPreConfigureAsync(context, cancellationToken);
    }

    /// <summary>
    /// Configures services required by the ASP.NET Core module.
    /// </summary>
    /// <param name="context">The configuration context for services.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        context.Services.AddLogging();
        ConfigureMiddlewares(context);
        ConfigureCoreServices(context);
        ConfigureExceptionHandlingServices(context);
        ConfigureExceptionHandling(context);
        
        // Configure endpoint routing options
        // Endpoint path is configurable via options (fixes hardcoded path issue)
        context.ConfigureOptions<BonEndpointRouterOptions>(options =>
        {
            // Register default module info endpoint
            // Note: Endpoints use DI directly, no need for EndpointRouteBuilderContext
            options.ConfigureActions.Add(endpoints =>
            {
                endpoints.MapGet("/bonyan/modules",
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
                            IsLoadedAsPlugIn = module.IsPluginModule,
                            AllAssemblies = module.AllAssemblies.Select(assembly => assembly.FullName).ToList(),
                            IsActive = module.Instance != null,
                            Timestamp = DateTime.UtcNow
                        });
                    });
            });
        });
        
        return base.OnConfigureAsync(context, cancellationToken);
    }

    public override ValueTask OnPostConfigureAsync(BonPostConfigurationContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        // Register UnitOfWork middleware conditionally after all modules are configured
        // This ensures we can check if IBonUnitOfWorkManager is registered
        if (context.Services.IsAdded<IBonUnitOfWorkManager>())
        {
            context.Services.TryAddTransient<BonyanUnitOfWorkMiddleware>();
        }

        return base.OnPostConfigureAsync(context, cancellationToken);
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
    /// Configures exception handling services (mappers, serializers).
    /// </summary>
    /// <param name="context">The configuration context for services.</param>
    private void ConfigureExceptionHandlingServices(BonConfigurationContext context)
    {
        // Register default exception mapper
        context.Services.TryAddSingleton<IExceptionToHttpResultMapper, DefaultExceptionToHttpResultMapper>();
        
        // Register default serializer (System.Text.Json)
        context.Services.TryAddSingleton<IExceptionResponseSerializer, SystemTextJsonExceptionResponseSerializer>();
    }

    /// <summary>
    /// Configures middleware dependencies for the application.
    /// </summary>
    /// <param name="context">The configuration context for services.</param>
    private void ConfigureMiddlewares(BonConfigurationContext context)
    {
        context.Services.AddTransient<BonyanClaimsMapMiddleware>();
        // Note: BonyanUnitOfWorkMiddleware is registered conditionally in OnPostConfigureAsync
        // after all modules are configured, to check if IBonUnitOfWorkManager is available
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

    public override ValueTask OnPreApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        context.Application.UseStaticFiles();
        return base.OnPreApplicationAsync(context, cancellationToken);
    }

    public override ValueTask OnApplicationAsync(BonWebApplicationContext webApplicationContext, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(webApplicationContext);
        cancellationToken.ThrowIfCancellationRequested();

        webApplicationContext.Application.UseHttpsRedirection();
        webApplicationContext.Application.UseAntiforgery();
        webApplicationContext.Application.UseRouting();
        return base.OnApplicationAsync(webApplicationContext, cancellationToken);
    }

    public override ValueTask OnPostApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        var options = context.Application.Services.GetService<IOptions<BonEndpointRouterOptions>>()?.Value;
        
        if (options == null || !options.ConfigureActions.Any())
        {
            return base.OnPostApplicationAsync(context, cancellationToken);
        }

        // Fix P0.2: Don't create scope here - endpoints create their own scopes per request
        // Store endpoint registrations and invoke them directly
        context.Application.UseEndpoints(endpoints =>
        {
            foreach (var configureAction in options.ConfigureActions)
            {
                configureAction(endpoints);
            }
        });
        
        return base.OnPostApplicationAsync(context, cancellationToken);
    }
}