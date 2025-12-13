using Bonyan.AspNetCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Factory for creating Bonyan modular web applications.
/// Part of the microkernel core - provides the entry point for web application bootstrapping.
/// </summary>
public static class BonyanApplication
{

    /// <summary>
    /// Creates a Bonyan application builder with the specified root module type.
    /// </summary>
    /// <typeparam name="TModule">The root module type.</typeparam>
    /// <param name="serviceKey">The service key (unique identifier, will be normalized). Required.</param>
    /// <param name="serviceTitle">The service title (display name). Required.</param>
    /// <param name="creationContext">Optional creation context.</param>
    /// <param name="args">Application arguments.</param>
    /// <returns>An instance of <see cref="IBonyanApplicationBuilder"/> configured with the root module.</returns>
    public static IBonyanApplicationBuilder CreateModularBuilder<TModule>(
        string serviceKey,
        string serviceTitle,
        Action<BonConfigurationContext>? creationContext = null,
        params string[] args)
        where TModule : IBonModule
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceKey, nameof(serviceKey));
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceTitle, nameof(serviceTitle));

        var applicationBuilder = WebApplication.CreateBuilder(args);
        
        // Configure Autofac if available (pluggable DI container)
        // TODO: Make this configurable via strategy pattern to support other containers
        // NOTE: ConfigureHostBuilder in .NET 6+ doesn't support IHostBuilder extensions directly
        // Autofac integration needs to be reworked for WebApplication builder
        // For now, using default service provider
        // try
        // {
        //     applicationBuilder.Host.UseBonAutofac();
        // }
        // catch
        // {
        //     // Autofac not available - use default service provider
        //     // This allows the framework to work without Autofac
        // }

        // Initialize the modular application and configure modules
        var modularApp = InitializeModularApplication<TModule>(
            applicationBuilder.Services,
            serviceKey,
            serviceTitle,
            creationContext);

        // Register core services for the modular application
        applicationBuilder.Services.AddSingleton<IWebBonModularityApplication>(modularApp);

        // Build the Bonyan application builder
        var builder = new BonyanApplicationBuilder(modularApp, applicationBuilder);
        
        // Initialize ServiceInfo with serviceKey (Id) and serviceTitle (Name)
        builder.ServiceInfo = new BonyanServiceInfo(
            serviceKey,
            serviceTitle,
            modularApp.GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0");

        return builder;
    }

    /// <summary>
    /// Creates a Bonyan application builder with the default ASP.NET Core module.
    /// </summary>
    /// <param name="serviceKey">The service key (unique identifier, will be normalized). Required.</param>
    /// <param name="serviceTitle">The service title (display name). Required.</param>
    /// <param name="creationContext">Optional creation context.</param>
    /// <param name="args">Application arguments.</param>
    /// <returns>An instance of <see cref="IBonyanApplicationBuilder"/>.</returns>
    public static IBonyanApplicationBuilder CreateBuilder(
        string serviceKey,
        string serviceTitle,
        Action<BonConfigurationContext>? creationContext = null,
        params string[] args)
    {
        return CreateModularBuilder<BonAspNetCoreModule>(serviceKey, serviceTitle, creationContext, args);
    }

    /// <summary>
    /// Initializes and configures the modular application.
    /// </summary>
    /// <typeparam name="TModule">The root module type.</typeparam>
    /// <param name="services">Service collection to register dependencies.</param>
    /// <param name="serviceKey">The service key (unique identifier).</param>
    /// <param name="serviceTitle">The service title (display name).</param>
    /// <param name="creationContext">Optional creation context.</param>
    /// <returns>An initialized instance of <see cref="WebBonModularityApplication{TModule}"/>.</returns>
    private static WebBonModularityApplication<TModule> InitializeModularApplication<TModule>(
        IServiceCollection services,
        string serviceKey,
        string serviceTitle,
        Action<BonConfigurationContext>? creationContext = null)
        where TModule : IBonModule
    {
        var modularApp = new WebBonModularityApplication<TModule>(services, serviceKey, serviceTitle, creationContext);

        return modularApp;
    }
}