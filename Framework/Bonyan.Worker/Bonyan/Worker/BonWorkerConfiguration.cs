using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bonyan.AspNetCore.Job;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Worker
{
    public class BonWorkerConfiguration
    {
        public BonWorkerConfiguration(IServiceCollection services, ServiceLifetime workerLifetime = ServiceLifetime.Transient)
        {
            Services = services;
            WorkerLifetime = workerLifetime;
        }

        public IServiceCollection Services { get; set; }
        public ServiceLifetime WorkerLifetime { get; set; }

        private readonly List<Assembly> _assemblies = new();
        private readonly List<Type> _workerTypes = new();
        private readonly List<WorkerRegistration> _workerRegistrations = new();

        /// <summary>
        /// Gets or sets a type filter predicate to include or exclude certain worker types.
        /// By default, all types are included.
        /// </summary>
        public Func<Type, bool> TypeFilter { get; set; } = type => true;

        /// <summary>
        /// Gets the type of the worker manager to use.
        /// </summary>
        internal Type WorkerManagerType { get; private set; } = typeof(InMemoryBonWorkerManager);

        /// <summary>
        /// Sets the worker manager to use.
        /// </summary>
        public BonWorkerConfiguration UseWorkerManager<TWorkerManager>() where TWorkerManager : class, IBonWorkerManager
        {
            WorkerManagerType = typeof(TWorkerManager);
            return this;
        }

        /// <summary>
        /// Registers all workers that implement <see cref="IBonWorker"/> from the specified assemblies.
        /// </summary>
        public BonWorkerConfiguration RegisterWorkersFromAssemblies(params Assembly[] assemblies)
        {
            _assemblies.AddRange(assemblies);
            return this;
        }

        /// <summary>
        /// Registers specific worker types directly.
        /// </summary>
        public BonWorkerConfiguration RegisterWorkersFromTypes(params Type[] types)
        {
            _workerTypes.AddRange(types);
            return this;
        }

        /// <summary>
        /// Registers a specific worker type using a generic method.
        /// </summary>
        public BonWorkerConfiguration RegisterWorker<TWorker>(ServiceLifetime? lifetime = null)
            where TWorker : class, IBonWorker
        {
            var workerType = typeof(TWorker);

            if (!_workerTypes.Contains(workerType))
            {
                _workerTypes.Add(workerType);
            }

            // Add specific registration with custom lifetime if provided
            RegisterWorkerType(workerType, lifetime ?? WorkerLifetime);

            return this;
        }

        /// <summary>
        /// Registers all discovered workers into the provided <see cref="IServiceCollection"/>.
        /// </summary>
        internal void RegisterWorkers()
        {
            // Collect all worker types from assemblies
            var assemblyWorkerTypes = _assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && ImplementsWorkerInterface(type))
                .Where(TypeFilter);

            // Register workers from assemblies
            foreach (var workerType in assemblyWorkerTypes)
            {
                RegisterWorkerType(workerType, WorkerLifetime);
            }

            // Register specific worker types
            foreach (var workerType in _workerTypes.Where(TypeFilter))
            {
                RegisterWorkerType(workerType, WorkerLifetime);
            }

            // Register all collected worker registrations into the IServiceCollection
            foreach (var registration in _workerRegistrations)
            {
                var descriptor = new ServiceDescriptor(registration.ImplementationType, registration.ImplementationType, registration.Lifetime);
                Services.Add(descriptor);
            }

            // Register the configuration so it can be accessed by the hosted service
            Services.AddSingleton(this);
        }

        /// <summary>
        /// Determines whether the specified type implements the <see cref="IBonWorker"/> interface.
        /// </summary>
        private static bool ImplementsWorkerInterface(Type type)
        {
            return typeof(IBonWorker).IsAssignableFrom(type);
        }

        /// <summary>
        /// Registers a specific worker type into the collection of worker registrations.
        /// </summary>
        private void RegisterWorkerType(Type workerType, ServiceLifetime lifetime)
        {
            // Avoid duplicate registrations
            if (!_workerRegistrations.Any(reg => reg.ImplementationType == workerType))
            {
                // Check for CronJobAttribute
                var cronAttribute = workerType.GetCustomAttribute<CronJobAttribute>();
                var cronExpression = cronAttribute?.CronExpression;

                _workerRegistrations.Add(new WorkerRegistration
                {
                    ImplementationType = workerType,
                    Lifetime = lifetime,
                    CronExpression = cronExpression // null if not a cron job
                });
            }
        }

        internal void RegisterWorkerManager()
        {
            // Check if IBonWorkerManager is already registered
            if (Services.All(service => service.ServiceType != typeof(IBonWorkerManager)))
            {
                Services.AddSingleton(typeof(IBonWorkerManager), WorkerManagerType);
            }
        }

        /// <summary>
        /// Represents a registration of a worker, including implementation type, lifetime, and cron expression.
        /// </summary>
        internal class WorkerRegistration
        {
            public Type ImplementationType { get; set; }
            public ServiceLifetime Lifetime { get; set; }
            public string CronExpression { get; set; }
        }

        /// <summary>
        /// Gets the collection of worker registrations.
        /// </summary>
        internal IEnumerable<WorkerRegistration> GetWorkerRegistrations()
        {
            return _workerRegistrations;
        }
    }
}
