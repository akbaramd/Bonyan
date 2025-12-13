using Bonyan.ExceptionHandling;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Reflection;
using Bonyan.UnitOfWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Fluent API extensions for configuring Bonyan applications.
/// Provides a design-system style fluent interface for consistent, discoverable configuration.
/// </summary>
public static class BonyanApplicationBuilderFluentExtensions
{
    /// <summary>
    /// Configures Bonyan web-specific options.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="configure">Action to configure Bonyan web options.</param>
    /// <returns>The builder for method chaining.</returns>
    public static IBonyanApplicationBuilder ConfigureBonyanWeb(
        this IBonyanApplicationBuilder builder,
        Action<BonyanWebOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new BonyanWebOptions();
        configure(options);

        // Apply configurations
        if (!string.IsNullOrEmpty(options.ModuleInfoPath))
        {
            builder.Services.Configure<BonEndpointRouterOptions>(endpointOptions =>
            {
                endpointOptions.ConfigureActions.Add(endpoints =>
                {
                    endpoints.MapGet(options.ModuleInfoPath!,
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
                                IsPlugin = module.IsPluginModule,
                                AllAssemblies = module.AllAssemblies.Select(a => a.FullName).ToList(),
                                IsActive = module.Instance != null
                            });
                        });
                });
            });
        }

        if (!string.IsNullOrEmpty(options.HealthCheckPath))
        {
            builder.Services.AddHealthChecks();
            builder.Services.Configure<BonEndpointRouterOptions>(endpointOptions =>
            {
                endpointOptions.ConfigureActions.Add(endpoints =>
                {
                    endpoints.MapHealthChecks(options.HealthCheckPath!);
                });
            });
        }

        if (options.UseProblemDetails)
        {
            builder.Services.AddProblemDetails();
        }

        if (options.UnitOfWorkOptions != null)
        {
            builder.Services.Configure<BonyanAspNetCoreUnitOfWorkOptions>(uowOptions =>
            {
                foreach (var ignoredUrl in options.UnitOfWorkOptions.IgnoredUrls)
                {
                    uowOptions.IgnoredUrls.Add(ignoredUrl);
                }
            });
        }

        return builder;
    }

    /// <summary>
    /// Configures endpoint routing.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="configure">Action to configure endpoints.</param>
    /// <returns>The builder for method chaining.</returns>
    public static IBonyanApplicationBuilder ConfigureEndpoints(
        this IBonyanApplicationBuilder builder,
        Action<IEndpointRouteBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        builder.Services.Configure<BonEndpointRouterOptions>(options =>
        {
            options.ConfigureActions.Add(configure);
        });

        return builder;
    }

    /// <summary>
    /// Configures exception handling.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="configure">Action to configure exception handling options.</param>
    /// <returns>The builder for method chaining.</returns>
    public static IBonyanApplicationBuilder ConfigureExceptionHandling(
        this IBonyanApplicationBuilder builder,
        Action<ExceptionHandlingOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        builder.Services.Configure(configure);
        return builder;
    }

    /// <summary>
    /// Configures Unit of Work options.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="configure">Action to configure Unit of Work options.</param>
    /// <returns>The builder for method chaining.</returns>
    public static IBonyanApplicationBuilder ConfigureUnitOfWork(
        this IBonyanApplicationBuilder builder,
        Action<BonyanAspNetCoreUnitOfWorkOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        builder.Services.Configure(configure);
        return builder;
    }
}

/// <summary>
/// Options for configuring Bonyan web application features.
/// </summary>
public class BonyanWebOptions
{
    /// <summary>
    /// Gets or sets the path for the module information endpoint.
    /// If null, the endpoint is not registered.
    /// </summary>
    public string? ModuleInfoPath { get; set; } = "/bonyan/modules";

    /// <summary>
    /// Gets or sets the path for the health check endpoint.
    /// If null, health checks are not configured.
    /// </summary>
    public string? HealthCheckPath { get; set; }

    /// <summary>
    /// Gets or sets whether to use ProblemDetails for error responses (RFC 7807).
    /// </summary>
    public bool UseProblemDetails { get; set; } = false;

    /// <summary>
    /// Gets or sets Unit of Work configuration options.
    /// </summary>
    public BonyanAspNetCoreUnitOfWorkOptions? UnitOfWorkOptions { get; set; }
}

