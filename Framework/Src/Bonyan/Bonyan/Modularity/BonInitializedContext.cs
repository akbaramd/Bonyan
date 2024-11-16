using Bonyan.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.Modularity
{
    /// <summary>
    /// Context for initializing services with configuration and dependency resolution capabilities.
    /// </summary>
    public class BonInitializedContext : BonContextBase
    {
        public BonInitializedContext(IServiceProvider services)
            : base(services)
        {
        }

        /// <summary>
        /// Asynchronously retrieves an optional configuration option.
        /// </summary>
        public async Task<T?> GetOptionAsync<T>() where T : class
        {
            var service = await Task.FromResult(GetService<IOptions<T>>());
            return service?.Value;
        }

        /// <summary>
        /// Retrieves a required configuration option, throwing an exception if validation fails.
        /// </summary>
        public T RequiredValidatedOption<T>(Func<T, bool> validate) where T : class, new()
        {
            var option = RequiredOption<T>();
            if (!validate(option))
            {
                throw new ConfigurationValidationException(typeof(T),
                    "Validation failed for required configuration option.");
            }

            return option;
        }
    }
}