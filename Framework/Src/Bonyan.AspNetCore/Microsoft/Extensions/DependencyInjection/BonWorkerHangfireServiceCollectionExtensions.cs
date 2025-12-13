using Bonyan.Modularity;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring Bonyan web application context.
/// </summary>
public static class BonServiceCollectionExtensions
{
    /// <summary>
    /// Configures the Bonyan web application context.
    /// </summary>
    /// <param name="application">The web application.</param>
    /// <param name="configure">Action to configure the application context.</param>
    /// <returns>The web application for method chaining.</returns>
    public static WebApplication UseBonyan(
        this WebApplication application,
        Action<BonWebApplicationContext> configure)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(configure);

        var context = new BonWebApplicationContext(application);
        configure.Invoke(context);
        return application;
    }
}