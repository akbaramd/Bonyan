using System.Reflection;
using Microsoft;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module = Autofac.Module;

namespace Bonyan.Modularity.Abstractions
{
    /// <summary>
    /// Base class for a module in the Bonyan modular system, providing methods for configuring dependencies.
    /// </summary>
    public abstract class BonModule : global::Autofac.Module, IBonModule
    {

        public IServiceCollection Services { get; set; } 
        
        public virtual Task OnPreConfigureAsync(BonConfigurationContext context) => Task.CompletedTask;

        public virtual Task OnConfigureAsync(BonConfigurationContext context) => Task.CompletedTask;

        public virtual Task OnPostConfigureAsync(BonConfigurationContext context) => Task.CompletedTask;

        public virtual Task OnPreInitializeAsync(ServiceInitializationContext context) => Task.CompletedTask;

        public virtual Task OnInitializeAsync(ServiceInitializationContext context) => Task.CompletedTask;

        public virtual Task OnPostInitializeAsync(ServiceInitializationContext context) => Task.CompletedTask;

        /// <summary>
        /// Checks if the provided type is a valid Bonyan module type.
        /// </summary>
        /// <param name="moduleType">The type to check.</param>
        /// <exception cref="ArgumentException">Thrown if the provided type is not a valid module type.</exception>
        internal static void CheckBonyanModuleType(Type moduleType)
        {
            if (!IsBonyanModule(moduleType))
            {
                throw new ArgumentException("The provided type is not a Bonyan module: " +
                                            moduleType.AssemblyQualifiedName);
            }
        }

        /// <summary>
        /// Determines if the provided type is a Bonyan module.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is a non-abstract, non-generic class that implements IModule; otherwise, false.</returns>
        public static bool IsBonyanModule(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsClass &&
                   !typeInfo.IsAbstract &&
                   typeof(IBonModule).GetTypeInfo().IsAssignableFrom(type);
        }

        /// <summary>
        /// List of dependent modules required by this module.
        /// </summary>
        public List<Type> DependedModules { get; set; } = new();

        /// <summary>
        /// Adds a dependency on the specified module type.
        /// </summary>
        /// <typeparam name="TModule">The module type to depend on.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the specified type is not a valid module type.</exception>
        public void DependOn<TModule>() where TModule : IBonModule
        {
            var moduleType = typeof(TModule);
            CheckBonyanModuleType(moduleType);

            if (!DependedModules.Contains(moduleType))
            {
                DependedModules.Add(moduleType);
            }
        }

        /// <summary>
        /// Adds dependencies on the specified module types.
        /// </summary>
        /// <param name="types">The types to depend on.</param>
        /// <exception cref="ArgumentException">Thrown if any specified type is not a valid module type.</exception>
        public void DependOn(params Type[] types)
        {
            if (types == null || types.Length == 0)
            {
                throw new ArgumentException("At least one module type must be specified.");
            }

            foreach (var type in types)
            {
                if (type == null)
                {
                    throw new ArgumentException("Module type cannot be null.");
                }

                CheckBonyanModuleType(type);

                if (!DependedModules.Contains(type))
                {
                    DependedModules.Add(type);
                }
            }
        }
        
        protected void Configure<TOptions>(Action<TOptions> configureOptions)
            where TOptions : class
        {
            Services.Configure(configureOptions);
        }

        protected void Configure<TOptions>(string name, Action<TOptions> configureOptions)
            where TOptions : class
        {
            Services.Configure(name, configureOptions);
        }

        protected void Configure<TOptions>(IConfiguration configuration)
            where TOptions : class
        {
            Services.Configure<TOptions>(configuration);
        }


        protected void Configure<TOptions>(string name, IConfiguration configuration)
            where TOptions : class
        {
            Services.Configure<TOptions>(name, configuration);
        }

        protected void PreConfigure<TOptions>(Action<TOptions> configureOptions)
            where TOptions : class
        {
            Services.PreConfigure(configureOptions);
        }

        protected void PostConfigure<TOptions>(Action<TOptions> configureOptions)
            where TOptions : class
        {
            Services.PostConfigure(configureOptions);
        }

        protected void PostConfigureAll<TOptions>(Action<TOptions> configureOptions)
            where TOptions : class
        {
            Services.PostConfigureAll(configureOptions);
        }
    }
}