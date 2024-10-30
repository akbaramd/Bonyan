using Bonyan.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Bonyan.Modularity;

public class ServiceInitializationContext : ServiceContextBase
{
    public IConfiguration Configuration { get; }

    public ServiceInitializationContext(IServiceProvider services, IConfiguration configuration)
        : base(services)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// Get a required configuration option, throwing an exception if not found.
    /// </summary>
    public T RequiredOption<T>() where T : class
    {
        var service = RequireService<IOptions<T>>();
        if (service.Value == null)
        {
            throw new ConfigurationNotFoundException<T>();
        }
        return service.Value;
    }

    /// <summary>
    /// Get an optional configuration option.
    /// </summary>
    public T? GetOption<T>() where T : class
    {
        var service = GetService<IOptions<T>>();
        return service?.Value;
    }
}