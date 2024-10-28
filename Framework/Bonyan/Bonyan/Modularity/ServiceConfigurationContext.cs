using System;
using Bonyan.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.Modularity;

public class ServiceConfigurationContext : ServiceContextBase
{
    public IServiceCollection Services { get; }
    public IConfiguration Configuration { get; }

    public ServiceConfigurationContext(IServiceCollection services, IConfiguration configuration)
        : base(services.BuildServiceProvider())
    {
        Services = services;
        Configuration = configuration;
    }

    /// <summary>
    /// Configure options of type TOptions.
    /// </summary>
    public void Configure<TOptions>(Action<TOptions> configureOptions) where TOptions : class
    {
        Services.Configure(configureOptions);
    }

    /// <summary>
    /// Configure named options of type TOptions.
    /// </summary>
    public void Configure<TOptions>(string name, Action<TOptions> configureOptions) where TOptions : class
    {
        Services.Configure(name, configureOptions);
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