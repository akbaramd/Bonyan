using Bonyan;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Hosting;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Represents a modular application with core dependencies and a modular application builder.
/// </summary>
public class BonyanApplication
{
    public WebApplication Application { get; }
    public BonyanServiceInfo ServiceInfo { get; init; }

    /// <summary>
    /// Initializes a new instance of <see cref="BonyanApplication"/>.
    /// </summary>
    /// <param name="application">The WebApplication instance.</param>
    /// <param name="serviceInfo">Information about the service being initialized.</param>
    public BonyanApplication(WebApplication application, BonyanServiceInfo serviceInfo)
    {
        Application = application ?? throw new ArgumentNullException(nameof(application));
        ServiceInfo = serviceInfo ?? throw new ArgumentNullException(nameof(serviceInfo));
    }

    /// <summary>
    /// Creates a Bonyan application builder with the specified root module type.
    /// </summary>
    /// <typeparam name="TModule">The root module type.</typeparam>
    /// <param name="args">Application arguments.</param>
    /// <returns>An instance of <see cref="IBonyanApplicationBuilder"/> configured with the root module.</returns>
    public static IBonyanApplicationBuilder CreateApplicationBuilder<TModule>(string[] args) where TModule : IModule
    {
        var applicationBuilder = WebApplication.CreateBuilder(args);

        // Initialize the modular application and configure modules
        var modularApp = InitializeModularApplication<TModule>(applicationBuilder.Services);

        // Register core services for the modular application
        applicationBuilder.Services.AddSingleton<IModularityApplication>(modularApp);
        applicationBuilder.Services.AddSingleton<IWebModularityApplication>(modularApp);

        // Build the Bonyan application builder with dependency injection support
        var builder = new BonyanApplicationBuilder(modularApp, applicationBuilder);
        builder.Host.UseAutofac();
        return builder;
    }

    /// <summary>
    /// Initializes and configures the modular application.
    /// </summary>
    /// <typeparam name="TModule">The root module type.</typeparam>
    /// <param name="services">Service collection to register dependencies.</param>
    /// <returns>An initialized instance of <see cref="WebModularityApplication{TModule}"/>.</returns>
    private static WebModularityApplication<TModule> InitializeModularApplication<TModule>(IServiceCollection services) where TModule : IModule
    {
        var modularApp = new WebModularityApplication<TModule>(services);
        
        // Asynchronously configure modules, handling potential errors
        try
        {
            modularApp.ConfigureModulesAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            // Log or handle errors during module configuration as needed
            // Example: logger.LogError(ex, "Module configuration failed.");
            throw new InvalidOperationException("Module configuration failed.", ex);
        }

        return modularApp;
    }
}
