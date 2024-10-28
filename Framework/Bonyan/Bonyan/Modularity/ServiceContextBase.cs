using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Modularity;

public abstract class ServiceContextBase
{
    protected IServiceProvider ServiceProvider { get; }

    protected ServiceContextBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Get an optional service from the ServiceProvider.
    /// </summary>
    public T? GetService<T>() where T : class
    {
        return ServiceProvider.GetService<T>();
    }

    /// <summary>
    /// Get a required service from the ServiceProvider.
    /// </summary>
    public T RequireService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }
}