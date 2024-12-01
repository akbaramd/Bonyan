using System.Reflection;
using Bonyan.Exceptions;
using Bonyan.Modularity;
using Bonyan.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft
{
    public static class ServiceConfigurationContextExtensions
    {
        /// <summary>
        /// Configures and validates options, ensuring that the configuration meets defined requirements.
        /// </summary>
        public static BonConfigurationContext ConfigureAndValidate<TOptions>(
            this BonConfigurationContext context,
            Action<TOptions> configureOptions,
            Func<TOptions, bool> validate) where TOptions : class, new()
        {
            context.Services.Configure<TOptions>(configureOptions);
            context.Services.PostConfigure<TOptions>(options =>
            {
                if (!validate(options))
                {
                    throw new ConfigurationValidationException(typeof(TOptions),
                        new List<string> { "Validation failed for configured options." });
                }
            });
            return context;
        }

        /// <summary>
        /// Registers a singleton service of type <typeparamref name="TService"/> with an optional implementation type.
        /// </summary>
        public static BonConfigurationContext RegisterSingletonService<TService>(
            this BonConfigurationContext context,
            Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
            context.Services.AddSingleton(implementationFactory);
            return context;
        }

        public static Assembly[] DiscoverApplicationAssemblies(this BonConfigurationContext context)
        {
            return context.Services.GetObjectOrNull<IAssemblyFinder>()?.Assemblies?.ToArray() ?? AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        /// Registers a transient service of type <typeparamref name="TService"/> with an optional implementation type.
        /// </summary>
        public static BonConfigurationContext RegisterTransientService<TService>(
            this BonConfigurationContext context,
            Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
            context.Services.AddTransient(implementationFactory);
            return context;
        }

        /// <summary>
        /// Registers a scoped service of type <typeparamref name="TService"/> with an optional implementation type.
        /// </summary>
        public static BonConfigurationContext RegisterScopedService<TService>(
            this BonConfigurationContext context,
            Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
            context.Services.AddScoped(implementationFactory);
            return context;
        }

        /// <summary>
        /// Registers an instance of a pre-created object as a singleton service.
        /// </summary>
        public static BonConfigurationContext RegisterSingletonInstance<TService>(
            this BonConfigurationContext context,
            TService instance) where TService : class
        {
            context.Services.AddSingleton(instance);
            return context;
        }

        /// <summary>
        /// Retrieves an optional configuration option or returns a default value if not found.
        /// </summary>
        public static T? GetOptionalOption<T>(
            this BonConfigurationContext context,
            T defaultValue = default) where T : class
        {
            var options = context.GetService<IOptions<T>>();
            return options?.Value ?? defaultValue;
        }

        /// <summary>
        /// Configures options with validation to ensure that the configuration meets the specified requirements.
        /// </summary>
        public static BonConfigurationContext ConfigureOptionsWithValidation<TOptions>(
            this BonConfigurationContext context,
            Action<TOptions> configureOptions,
            Func<TOptions, IEnumerable<string>> validate) where TOptions : class, new()
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

        // Singleton Services
        public static BonConfigurationContext RegisterSingletonServicesFor<TInterface>(
            this BonConfigurationContext context)
        {
            return context.RegisterSingletonServicesFor(typeof(TInterface));
        }

        public static BonConfigurationContext RegisterSingletonServicesFor(
            this BonConfigurationContext context,
            Type interfaceType)
        {
            return RegisterServicesFor(
                context,
                ServiceLifetime.Singleton,
                type => ImplementsInterface(type, interfaceType) && type.IsClass && !type.IsAbstract
            );
        }

        public static BonConfigurationContext RegisterSingletonServicesFor(
            this BonConfigurationContext context,
            Func<Type, bool> condition)
        {
            return RegisterServicesFor(context, ServiceLifetime.Singleton, condition);
        }

        // Scoped Services
        public static BonConfigurationContext RegisterScopedServicesFor<TInterface>(
            this BonConfigurationContext context)
        {
            return context.RegisterScopedServicesFor(typeof(TInterface));
        }

        public static BonConfigurationContext RegisterScopedServicesFor(
            this BonConfigurationContext context,
            Type interfaceType)
        {
            return RegisterServicesFor(
                context,
                ServiceLifetime.Scoped,
                type => ImplementsInterface(type, interfaceType) && type.IsClass && !type.IsAbstract
            );
        }

        public static BonConfigurationContext RegisterScopedServicesFor(
            this BonConfigurationContext context,
            Func<Type, bool> condition)
        {
            return RegisterServicesFor(context, ServiceLifetime.Scoped, condition);
        }

        // Transient Services
        public static BonConfigurationContext RegisterTransientServicesFor<TInterface>(
            this BonConfigurationContext context)
        {
            return context.RegisterTransientServicesFor(typeof(TInterface));
        }

        public static BonConfigurationContext RegisterTransientServicesFor(
            this BonConfigurationContext context,
            Type interfaceType)
        {
            return RegisterServicesFor(
                context,
                ServiceLifetime.Transient,
                type => ImplementsInterface(type, interfaceType) && type.IsClass && !type.IsAbstract
            );
        }

        public static BonConfigurationContext RegisterTransientServicesFor(
            this BonConfigurationContext context,
            Func<Type, bool> condition)
        {
            return RegisterServicesFor(context, ServiceLifetime.Transient, condition);
        }

        // Services by Type
        public static BonConfigurationContext RegisterSingletonServicesOf<TType>(this BonConfigurationContext context)
        {
            return context.RegisterSingletonServicesOf(typeof(TType));
        }

        public static BonConfigurationContext RegisterSingletonServicesOf(this BonConfigurationContext context, Type typeToRegister)
        {
            return RegisterServicesOf(context, ServiceLifetime.Singleton, 
                type => type.IsClass && !type.IsAbstract && typeToRegister.IsAssignableFrom(type));
        }
        public static BonConfigurationContext RegisterSingletonServicesOf(this BonConfigurationContext context,
            Func<Type, bool> condition)
        {
            return RegisterServicesOf(context, ServiceLifetime.Singleton, condition);
        }

        public static BonConfigurationContext RegisterScopedServicesOf<TType>(this BonConfigurationContext context)
        {
            return context.RegisterScopedServicesOf(typeof(TType));
        }

        public static BonConfigurationContext RegisterScopedServicesOf(this BonConfigurationContext context, Type typeToRegister)
        {
            return RegisterServicesOf(context, ServiceLifetime.Scoped, 
                type => type.IsClass && !type.IsAbstract && typeToRegister.IsAssignableFrom(type));
        }

        public static BonConfigurationContext RegisterScopedServicesOf(this BonConfigurationContext context,
            Func<Type, bool> condition)
        {
            return RegisterServicesOf(context, ServiceLifetime.Scoped, condition);
        }

        public static BonConfigurationContext RegisterTransientServicesOf<TType>(this BonConfigurationContext context)
        {
            return context.RegisterTransientServicesOf(typeof(TType));
        }

        public static BonConfigurationContext RegisterTransientServicesOf(this BonConfigurationContext context, Type typeToRegister)
        {
            return RegisterServicesOf(context, ServiceLifetime.Transient, 
                type => type.IsClass && !type.IsAbstract && typeToRegister.IsAssignableFrom(type));
        }

        public static BonConfigurationContext RegisterTransientServicesOf(this BonConfigurationContext context,
            Func<Type, bool> condition)
        {
            return RegisterServicesOf(context, ServiceLifetime.Transient, condition);
        }

        // Core Logic for Adding Services by Type
        public static BonConfigurationContext RegisterServicesOf(
            BonConfigurationContext context,
            ServiceLifetime lifetime,
            Func<Type, bool> condition)
        {
            var assemblies = context.DiscoverApplicationAssemblies().Distinct();
            var matchingTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(condition);

            foreach (var serviceType in matchingTypes)
            {
                switch (lifetime)
                {
                    case ServiceLifetime.Singleton:
                        context.Services.AddSingleton(serviceType);
                        break;
                    case ServiceLifetime.Scoped:
                        context.Services.AddScoped(serviceType);
                        break;
                    case ServiceLifetime.Transient:
                        context.Services.AddTransient(serviceType);
                        break;
                }
            }

            return context;
        }

        public static BonConfigurationContext RegisterServicesFor(
            BonConfigurationContext context,
            ServiceLifetime lifetime,
            Func<Type, bool> condition)
        {
            var assemblies = context.DiscoverApplicationAssemblies().Distinct();
            var serviceTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(condition);

            foreach (var serviceType in serviceTypes)
            {
                foreach (var interfaceType in serviceType.GetInterfaces())
                {
                    switch (lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            context.Services.AddSingleton(interfaceType, serviceType);
                            break;
                        case ServiceLifetime.Scoped:
                            context.Services.AddScoped(interfaceType, serviceType);
                            break;
                        case ServiceLifetime.Transient:
                            context.Services.AddTransient(interfaceType, serviceType);
                            break;
                    }
                }
            }

            return context;
        }

        private static bool ImplementsInterface(Type type, Type interfaceType)
        {
            if (interfaceType.IsGenericTypeDefinition)
            {
                return type.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
            }

            return interfaceType.IsAssignableFrom(type);
        }
    }
}
