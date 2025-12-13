using System.Reflection;
using Bonyan.DependencyInjection;
using Bonyan.Modularity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.Modularity.Abstractions
{
    /// <summary>
    /// Base class for a module in the Bonyan modular system, providing methods for configuring dependencies.
    /// </summary>
    public abstract class BonModule : global::Autofac.Module, IBonModule
    {
        /// <summary>
        /// Service collection for registering module dependencies.
        /// </summary>
        public IServiceCollection Services { get; set; }

        /// <summary>
        /// List of dependent modules required by this module.
        /// </summary>
        public List<Type> DependedModules { get; set; } = new();

        /// <summary>
        /// Tracks whether the object has already been disposed.
        /// </summary>
        private bool _disposed;

        public virtual ValueTask OnPreConfigureAsync(BonPreConfigurationContext context, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;
        public virtual ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;
        public virtual ValueTask OnPostConfigureAsync(BonPostConfigurationContext context, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;
        public virtual ValueTask OnPreInitializeAsync(BonInitializedContext context, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;
        public virtual ValueTask OnInitializeAsync(BonInitializedContext context, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;
        public virtual ValueTask OnPostInitializeAsync(BonInitializedContext context, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;
        public virtual ValueTask OnShutdownAsync(BonShutdownContext context, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

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

        // Note: PreConfigure, Configure, and PostConfigure methods are now available through their respective contexts:
        // - PreConfigure: Use context.PreConfigure<TOptions>() in OnPreConfigureAsync
        // - Configure: Use context.ConfigureOptions<TOptions>() in OnConfigureAsync
        // - PostConfigure: Use context.PostConfigure<TOptions>() in OnPostConfigureAsync
        // This ensures proper phase separation and prevents misuse.
        /// <summary>
        /// Disposes of the resources used by this module.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">Indicates whether the method was called directly or indirectly.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources here
                    Services = null;
                    DependedModules.Clear();
                }

                // Dispose unmanaged resources here, if any

                _disposed = true;
            }
        }

        /// <summary>
        /// Destructor for final cleanup.
        /// </summary>
        ~BonModule()
        {
            Dispose(false);
        }
    }
}