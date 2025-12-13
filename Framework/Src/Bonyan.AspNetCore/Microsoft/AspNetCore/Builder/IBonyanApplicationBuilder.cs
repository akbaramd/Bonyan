using Bonyan.Modularity;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Defines the contract for building Bonyan modular web applications.
/// </summary>
public interface IBonyanApplicationBuilder
{
    /// <summary>
    /// Gets the configuration manager.
    /// </summary>
    IConfigurationManager Configuration { get; }

    /// <summary>
    /// Gets the host environment.
    /// </summary>
    IHostEnvironment Environment { get; }

    /// <summary>
    /// Gets the logging builder.
    /// </summary>
    ILoggingBuilder Logging { get; }

    /// <summary>
    /// Gets the service collection.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Gets the host builder.
    /// </summary>
    IHostBuilder Host { get; }

    /// <summary>
    /// Gets or sets the service information.
    /// </summary>
    BonyanServiceInfo? ServiceInfo { get; set; }

    /// <summary>
    /// Builds and initializes the modular ASP.NET Core application.
    /// </summary>
    /// <param name="applicationContext">Optional action to configure the application context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The built and initialized web application.</returns>
    Task<WebApplication> BuildAsync(
        Action<BonWebApplicationContext>? applicationContext = null,
        CancellationToken cancellationToken = default);
}
