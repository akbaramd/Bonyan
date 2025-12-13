namespace Bonyan.Modularity.Abstractions;

/// <summary>
/// Defines the contract for initializing web applications with modularity support.
/// </summary>
public interface IWebBonModularityApplication : IBonModularityApplication
{
    /// <summary>
    /// Initializes the web application by executing web module lifecycle phases.
    /// </summary>
    /// <param name="application">The web application instance.</param>
    /// <param name="applicationContext">Optional action to configure the application context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task InitializeApplicationAsync(
        WebApplication application,
        Action<BonWebApplicationContext>? applicationContext = null,
        CancellationToken cancellationToken = default);
}
