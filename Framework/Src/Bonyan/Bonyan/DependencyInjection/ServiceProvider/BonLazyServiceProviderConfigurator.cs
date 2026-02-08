namespace Bonyan.DependencyInjection;

/// <summary>
/// Implementation of <see cref="IBonLazyServiceProviderConfigurator"/> for managing and accessing services within a lazy-loaded service provider context.
/// </summary>
public class BonLazyServiceProviderConfigurator : IBonLazyServiceProviderConfigurator
{
    /// <inheritdoc />
    public IBonLazyServiceProvider LazyServiceProvider { get; set; } = default!;

    /// <inheritdoc />
    public object? GetService(Type serviceType) => LazyServiceProvider.GetService(serviceType);

    /// <inheritdoc />
    public T GetService<T>(T defaultValue) => LazyServiceProvider.GetService(defaultValue);

    /// <inheritdoc />
    public object GetService(Type serviceType, object defaultValue) => LazyServiceProvider.GetService(serviceType, defaultValue);

    /// <inheritdoc />
    public T GetService<T>(Func<IServiceProvider, object> factory) => LazyServiceProvider.GetService<T>(factory);

    /// <inheritdoc />
    public object GetService(Type serviceType, Func<IServiceProvider, object> factory) => LazyServiceProvider.GetService(serviceType, factory);

    /// <inheritdoc />
    public T LazyGetRequiredService<T>() => LazyServiceProvider.LazyGetRequiredService<T>();

    /// <inheritdoc />
    public object LazyGetRequiredService(Type serviceType) => LazyServiceProvider.LazyGetRequiredService(serviceType);

    /// <inheritdoc />
    public T? LazyGetService<T>() => LazyServiceProvider.LazyGetService<T>();

    /// <inheritdoc />
    public object? LazyGetService(Type serviceType) => LazyServiceProvider.LazyGetService(serviceType);

    /// <inheritdoc />
    public T LazyGetService<T>(T defaultValue) => LazyServiceProvider.LazyGetService(defaultValue);

    /// <inheritdoc />
    public object LazyGetService(Type serviceType, object defaultValue) => LazyServiceProvider.LazyGetService(serviceType, defaultValue);

    /// <inheritdoc />
    public object LazyGetService(Type serviceType, Func<IServiceProvider, object> factory) => LazyServiceProvider.LazyGetService(serviceType, factory);

    /// <inheritdoc />
    public T LazyGetService<T>(Func<IServiceProvider, object> factory) => LazyServiceProvider.LazyGetService<T>(factory);
}
