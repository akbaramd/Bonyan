using Bonyan.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.Modularity
{
    /// <summary>
    /// Base context class that provides shared utilities for managing services and configuration options.
    /// </summary>
    public abstract class BonContextBase
    {
        /// <summary>
        /// Gets the application's service provider.
        /// </summary>
        protected internal IServiceProvider Services { get; }

        /// <summary>
        /// Gets the application's configuration settings.
        /// </summary>
        protected internal IConfiguration Configuration { get; }

        protected BonContextBase(IServiceProvider services)
        {
            Services = services;
            Configuration = Services.GetService<IConfiguration>() ?? new ConfigurationBuilder().Build();
        }

        /// <summary>
        /// Retrieves a required configuration option, throwing an exception if not found.
        /// </summary>
        public T RequiredOption<T>() where T : class
        {
            var service = Services.GetRequiredService<IOptions<T>>();
            if (service.Value == null)
            {
                throw new ConfigurationNotFoundException<T>();
            }
            return service.Value;
        }

        /// <summary>
        /// Retrieves an optional configuration option, returning null if not found.
        /// </summary>
        public T? GetOption<T>() where T : class
        {
            var service = Services.GetService<IOptions<T>>();
            return service?.Value;
        }

        /// <summary>
        /// Attempts to retrieve a service, returning null if not found.
        /// </summary>
        public T? GetService<T>() where T : class
        {
            return Services.GetService<T>();
        }

        /// <summary>
        /// Retrieves a required service, throwing an exception if not found.
        /// </summary>
        public T RequireService<T>() where T : notnull
        {
            return Services.GetRequiredService<T>();
        }
    }
}
