namespace Bonyan.Modularity.Abstractions;

/// <summary>
/// Defines the contract for web modules that participate in the ASP.NET Core application lifecycle.
/// </summary>
public interface IWebModule : IBonModule
{
    /// <summary>
    /// Called before the middleware pipeline is configured.
    /// Use this phase to configure static files, CORS, etc.
    /// </summary>
    /// <param name="context">The web application context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    ValueTask OnPreApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Called to configure the middleware pipeline.
    /// Use this phase to configure routing, authentication, etc.
    /// </summary>
    /// <param name="context">The web application context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    ValueTask OnApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Called after the middleware pipeline is configured.
    /// Use this phase to configure endpoints.
    /// </summary>
    /// <param name="context">The web application context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    ValueTask OnPostApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default);
}
