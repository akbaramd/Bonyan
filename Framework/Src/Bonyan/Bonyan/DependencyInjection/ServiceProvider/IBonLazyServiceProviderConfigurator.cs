namespace Bonyan.DependencyInjection;

/// <summary>
/// Configurator for lazy service resolution; provides access to services within a lazy-loaded context.
/// </summary>
public interface IBonLazyServiceProviderConfigurator : IBonLazyServiceProvider
{
    /// <summary>Gets or sets the lazy service provider used for resolving services.</summary>
    IBonLazyServiceProvider LazyServiceProvider { get; set; }
}
