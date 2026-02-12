namespace Bonyan.DependencyInjection;

/// <summary>
/// Service to inject properties on an instance using the current container (e.g. Autofac).
/// </summary>
public interface IInjectPropertiesService
{
    /// <summary>
    /// Injects properties on the given instance.
    /// </summary>
    TService InjectProperties<TService>(TService instance) where TService : notnull;

    /// <summary>
    /// Injects only properties that are not already set on the given instance.
    /// </summary>
    TService InjectUnsetProperties<TService>(TService instance) where TService : notnull;
}
