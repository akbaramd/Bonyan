using Bonyan.Exceptions;
using Bonyan.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft
{
    public static class ServiceConfigurationContextExtensions
    {
   
        /// <summary>
        /// Configures and validates options, ensuring that the configuration meets defined requirements.
        /// </summary>
        public static BonConfigurationContext ConfigureAndValidate<TOptions>(this BonConfigurationContext context, Action<TOptions> configureOptions, Func<TOptions, bool> validate) where TOptions : class, new()
        {
            context.Services.Configure<TOptions>(configureOptions);
            context.Services.PostConfigure<TOptions>(options =>
            {
                if (!validate(options))
                {
                    throw new ConfigurationValidationException(typeof(TOptions), new List<string> { "Validation failed for configured options." });
                }
            });
            return context;
        }

        /// <summary>
        /// Registers a singleton service of type <typeparamref name="TService"/> with an optional implementation type.
        /// </summary>
        public static BonConfigurationContext AddSingletonService<TService>(this BonConfigurationContext context, Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
            context.Services.AddSingleton(implementationFactory);
            return context;
        }

        /// <summary>
        /// Registers a transient service of type <typeparamref name="TService"/> with an optional implementation type.
        /// </summary>
        public static BonConfigurationContext AddTransientService<TService>(this BonConfigurationContext context, Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
            context.Services.AddTransient(implementationFactory);
            return context;
        }

        /// <summary>
        /// Registers a scoped service of type <typeparamref name="TService"/> with an optional implementation type.
        /// </summary>
        public static BonConfigurationContext AddScopedService<TService>(this BonConfigurationContext context, Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
            context.Services.AddScoped(implementationFactory);
            return context;
        }

        /// <summary>
        /// Registers an instance of a pre-created object as a singleton service.
        /// </summary>
        public static BonConfigurationContext AddSingletonInstance<TService>(this BonConfigurationContext context, TService instance) where TService : class
        {
            context.Services.AddSingleton(instance);
            return context;
        }

        /// <summary>
        /// Retrieves an optional configuration option or returns a default value if not found.
        /// </summary>
        public static T? GetOptionalOption<T>(this BonConfigurationContext context, T defaultValue = default) where T : class
        {
            var options = context.GetService<IOptions<T>>();
            return options?.Value ?? defaultValue;
        }

        /// <summary>
        /// Configures options with validation to ensure that the configuration meets the specified requirements.
        /// </summary>
        public static BonConfigurationContext ConfigureOptionsWithValidation<TOptions>(this BonConfigurationContext context, Action<TOptions> configureOptions, Func<TOptions, IEnumerable<string>> validate) where TOptions : class, new()
        {
            context.Services.Configure<TOptions>(configureOptions);
            context.Services.PostConfigure<TOptions>(options =>
            {
                var errors = validate(options);
                if (errors != null && errors.Any())
                {
                    throw new ConfigurationValidationException(typeof(TOptions), errors);
                }
            });
            return context;
        }
    }
}
