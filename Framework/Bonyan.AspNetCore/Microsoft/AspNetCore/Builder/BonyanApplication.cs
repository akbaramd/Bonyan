using Bonyan;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Hosting;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Represents a modular application with core dependencies and a modular application builder.
/// </summary>
public class BonyanApplication
{
    public WebApplication Application { get; }
    public BonyanServiceInfo ServiceInfo { get; private set;}

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
    public static IBonyanApplicationBuilder CreateModularApplication<TModule>(string serviceName ,params string[] args) where TModule : IBonModule
    {
        
        
        
        var applicationBuilder = WebApplication.CreateBuilder(args);
        applicationBuilder.Host.UseBonAutofac();
        
        // Store the service name in a shared configuration
        applicationBuilder.Services.AddObjectAccessor<BonyanServiceOptions>(new BonyanServiceOptions()
        {
            ServiceName = serviceName
        });
        
        // Initialize the modular application and configure modules
        var modularApp = InitializeModularApplication<TModule>(applicationBuilder.Services);

        // Register core services for the modular application
        applicationBuilder.Services.AddSingleton<IBonModularityApplication>(modularApp);
        applicationBuilder.Services.AddSingleton<IWebBonModularityApplication>(modularApp);

      
        // Build the Bonyan application builder with dependency injection support
        var builder = new BonyanApplicationBuilder(modularApp, applicationBuilder);
        return builder;
    }

    /// <summary>
    /// Initializes and configures the modular application.
    /// </summary>
    /// <typeparam name="TModule">The root module type.</typeparam>
    /// <param name="services">Service collection to register dependencies.</param>
    /// <returns>An initialized instance of <see cref="WebBonModularityApplication{TModule}"/>.</returns>
    private static WebBonModularityApplication<TModule> InitializeModularApplication<TModule>(IServiceCollection services) where TModule : IBonModule
    {
        var modularApp = new WebBonModularityApplication<TModule>(services);
        
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
