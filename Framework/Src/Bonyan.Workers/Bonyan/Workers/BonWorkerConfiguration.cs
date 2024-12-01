using System.Reflection;
using Bonyan.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Workers
{
    public class BonWorkerConfiguration
    {
        public BonWorkerConfiguration(BonConfigurationContext context, ServiceLifetime workerLifetime = ServiceLifetime.Transient)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            WorkerLifetime = workerLifetime;

            // Register the configuration so it can be accessed by the hosted service
            Context.Services.AddSingleton(this);
        }

        public BonConfigurationContext Context { get; set; }
        public ServiceLifetime WorkerLifetime { get; set; }

        private readonly HashSet<Type> _registeredWorkerTypes = new HashSet<Type>();

        /// <summary>
        /// Registers all workers that implement <see cref="IBonWorker"/> from the specified assemblies.
        /// </summary>
        public BonWorkerConfiguration RegisterWorkersFromAssemblies(params Assembly[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            // Collect all worker types from assemblies and register them immediately
            var workerTypes = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => IsConcreteClass(type) && ImplementsWorkerInterface(type));

            foreach (var workerType in workerTypes)
            {
                RegisterWorkerType(workerType, WorkerLifetime);
            }

            return this;
        }

        /// <summary>
        /// Registers specific worker types directly.
        /// </summary>
        public BonWorkerConfiguration RegisterWorkersFromTypes(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            var workerTypes = types
                .Where(type => IsConcreteClass(type) && ImplementsWorkerInterface(type));

            foreach (var workerType in workerTypes)
            {
                RegisterWorkerType(workerType, WorkerLifetime);
            }

            return this;
        }

        /// <summary>
        /// Registers a specific worker type using a generic method.
        /// </summary>
        public BonWorkerConfiguration RegisterWorker<TWorker>(ServiceLifetime? lifetime = null)
            where TWorker : class, IBonWorker
        {
            var workerType = typeof(TWorker);

            // Register the worker immediately
            RegisterWorkerType(workerType, lifetime ?? WorkerLifetime);

            return this;
        }

        /// <summary>
        /// Determines whether the specified type is a concrete class.
        /// </summary>
        private static bool IsConcreteClass(Type type)
        {
            return type.IsClass && !type.IsAbstract;
        }

        /// <summary>
        /// Determines whether the specified type implements the <see cref="IBonWorker"/> interface.
        /// </summary>
        private static bool ImplementsWorkerInterface(Type type)
        {
            return typeof(IBonWorker).IsAssignableFrom(type);
        }

        /// <summary>
        /// Registers a specific worker type into the IServiceCollection and stores it in the collection.
        /// </summary>
        private void RegisterWorkerType(Type workerType, ServiceLifetime lifetime)
        {
            // Check if the worker type is already registered
            if (!_registeredWorkerTypes.Contains(workerType))
            {
                // Register the worker in the DI container
                var descriptor = new ServiceDescriptor(workerType, workerType, lifetime);
                Context.Services.Add(descriptor);

                // Add to the collection of registered worker types
                _registeredWorkerTypes.Add(workerType);
            }
        }

        /// <summary>
        /// Gets the collection of registered worker types.
        /// </summary>
        public IEnumerable<Type> GetRegisteredWorkerTypes()
        {
            return _registeredWorkerTypes;
        }
    }
}
