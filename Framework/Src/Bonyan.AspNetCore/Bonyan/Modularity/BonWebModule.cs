using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

/// <summary>
/// Base class for web modules that participate in the ASP.NET Core application lifecycle.
/// </summary>
public abstract class BonWebModule : BonModule, IWebModule
{
    /// <summary>
    /// Called before the middleware pipeline is configured.
    /// </summary>
    public virtual ValueTask OnPreApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Called to configure the middleware pipeline.
    /// </summary>
    public virtual ValueTask OnApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Called after the middleware pipeline is configured.
    /// </summary>
    public virtual ValueTask OnPostApplicationAsync(BonWebApplicationContext context, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }
}