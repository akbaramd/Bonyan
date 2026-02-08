namespace Bonyan.DependencyInjection;

/// <summary>
/// Provides access to the root <see cref="IServiceProvider"/> (e.g. from the application).
/// </summary>
public interface IBonServiceProviderAccessor
{
    /// <summary>Root service provider.</summary>
    IServiceProvider ServiceProvider { get; }
}
