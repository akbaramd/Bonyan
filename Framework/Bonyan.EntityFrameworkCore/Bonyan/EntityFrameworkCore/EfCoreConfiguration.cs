using System.Reflection;
using Bonyan.DDD.Application;
using Bonyan.DDD.Domain;
using Bonyan.DDD.Domain.Abstractions;
using Bonyan.DDD.Domain.Entities;
using Bonyan.EntityFrameworkCore.HostedServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bonyan.EntityFrameworkCore
{
    public class EfCoreConfiguration<TDbContext> where TDbContext : DbContext
    {
        public EfCoreConfiguration(IServiceCollection services)
        {
            Services = services;
            RegisterRepositories();
        }

        public IServiceCollection Services { get; }
        public Action<DbContextOptionsBuilder> DbContextOptionsAction { get; set; } = _ => { };

        private void RegisterRepositories()
        {
            var dbContextType = typeof(TDbContext);
            var entityType = typeof(IEntity); // Base IEntity interface without key
            var genericEntityType = typeof(IEntity<>); // IEntity<TKey> interface with key
            var assembly = Assembly.GetExecutingAssembly(); // Or the assembly where your entities are located

            // Get all types that implement IEntity or IEntity<TKey>
            var entityTypes = assembly.GetTypes()
                .Where(t => !t.IsInterface && !t.IsAbstract &&
                            (entityType.IsAssignableFrom(t) || t.GetInterfaces()
                                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericEntityType)));

            foreach (var type in entityTypes)
            {
                // Check if the type implements IEntity<TKey>
                var entityInterface = type.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericEntityType);

                if (entityInterface != null)
                {
                    // Get the TKey type from IEntity<TKey>
                    var keyType = entityInterface.GetGenericArguments()[0];

                    // Ensure the key type is valid
                    if (keyType.IsAbstract || keyType.IsInterface)
                    {
                        throw new InvalidOperationException($"The key type '{keyType.Name}' for entity '{type.Name}' is not a valid type. It cannot be an abstract class or interface.");
                    }

                    // Register IRepository<TEntity, TKey> with EfCoreRepository<TEntity, TKey, TDbContext>
                    var repositoryWithKeyType = typeof(EfCoreRepository<,,>).MakeGenericType(type, keyType, dbContextType);
                    var repositoryWithKeyInterfaceType = typeof(IRepository<,>).MakeGenericType(type, keyType);
                    Services.AddScoped(repositoryWithKeyInterfaceType, repositoryWithKeyType);
                }
                else
                {
                    // Register IRepository<TEntity> with EfCoreRepository<TEntity, TDbContext>
                    var repositoryType = typeof(EfCoreRepository<,>).MakeGenericType(type, dbContextType);
                    var repositoryInterfaceType = typeof(IRepository<>).MakeGenericType(type);
                    Services.AddScoped(repositoryInterfaceType, repositoryType);
                }
            }

            // Register IUnitOfWork with EfCoreUnitOfWork<TDbContext>
            Services.AddScoped(typeof(IUnitOfWork), typeof(EfCoreUnitOfWork<TDbContext>));
        }

        public EfCoreConfiguration<TDbContext> EnsureDatabaseCreated<TContext>()
            where TContext : DbContext
        {
            Services.AddScoped<IHostedService, EnsureDatabaseCreatedService<TContext>>();
            return this;
        }

        public EfCoreConfiguration<TDbContext> ApplyMigrations<TContext>()
            where TContext : DbContext
        {
            Services.AddScoped<IHostedService, ApplyMigrationsService<TContext>>();
            return this;
        }

        public EfCoreConfiguration<TDbContext> ConfigureLogging<TContext>(Action<ILoggingBuilder> configureLogging)
            where TContext : DbContext
        {
            Services.AddDbContext<TContext>((serviceProvider, options) =>
            {
                configureLogging(serviceProvider.GetRequiredService<ILoggingBuilder>());
            });

            return this;
        }

        public EfCoreConfiguration<TDbContext> AddRepository<TRepositoryInterface>()
        {
            // Get the type of TRepositoryInterface
            var repositoryInterfaceType = typeof(TRepositoryInterface);

            if (!repositoryInterfaceType.IsInterface)
            {
                throw new InvalidOperationException($"{repositoryInterfaceType.Name} is not an interface.");
            }

            // Check if TRepositoryInterface implements IRepository<TEntity, TKey>
            var repositoryWithKeyInterfaceType = repositoryInterfaceType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<,>));

            if (repositoryWithKeyInterfaceType != null)
            {
                // Extract TEntity and TKey types from IRepository<TEntity, TKey>
                var entityType = repositoryWithKeyInterfaceType.GetGenericArguments()[0];
                var keyType = repositoryWithKeyInterfaceType.GetGenericArguments()[1];

                // Validate that entityType and keyType are valid types
                if (entityType == null || keyType == null)
                {
                    throw new InvalidOperationException($"Unable to extract entity or key type from {repositoryInterfaceType.Name}.");
                }

                // Create the implementation type EfCoreRepository<TEntity, TKey, TDbContext>
                var repositoryImplementationType = typeof(EfCoreRepository<,,>).MakeGenericType(entityType, keyType, typeof(TDbContext));

                // Register TRepositoryInterface with the EfCoreRepository<TEntity, TKey, TDbContext>
                Services.AddScoped(repositoryInterfaceType, repositoryImplementationType);
            }
            else
            {
                // Check if TRepositoryInterface implements IRepository<TEntity>
                var repositoryWithoutKeyInterfaceType = repositoryInterfaceType.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>));

                if (repositoryWithoutKeyInterfaceType != null)
                {
                    // Extract TEntity type from IRepository<TEntity>
                    var entityType = repositoryWithoutKeyInterfaceType.GetGenericArguments()[0];

                    // Validate that entityType is a valid type
                    if (entityType == null)
                    {
                        throw new InvalidOperationException($"Unable to extract entity type from {repositoryInterfaceType.Name}.");
                    }

                    // Create the implementation type EfCoreRepository<TEntity, TDbContext>
                    var repositoryImplementationType = typeof(EfCoreRepository<,>).MakeGenericType(entityType, typeof(TDbContext));

                    // Register TRepositoryInterface with the EfCoreRepository<TEntity, TDbContext>
                    Services.AddScoped(repositoryInterfaceType, repositoryImplementationType);
                }
                else
                {
                    throw new InvalidOperationException($"The provided repository interface {repositoryInterfaceType.Name} must implement IRepository<TEntity, TKey> or IRepository<TEntity>.");
                }
            }

            return this;
        }

        public EfCoreConfiguration<TDbContext> AddRepository<TRepositoryInterface, TRepositoryImpl>()
            where TRepositoryInterface : class // Ensures TRepositoryInterface is a reference type
            where TRepositoryImpl : class, TRepositoryInterface // Ensures TRepositoryImpl implements TRepositoryInterface
        {
            // Get the types for validation
            var repositoryInterfaceType = typeof(TRepositoryInterface);
            var repositoryImplementationType = typeof(TRepositoryImpl);

            if (!repositoryInterfaceType.IsInterface)
            {
                throw new InvalidOperationException($"{repositoryInterfaceType.Name} is not an interface.");
            }

            if (!repositoryImplementationType.IsClass)
            {
                throw new InvalidOperationException($"{repositoryImplementationType.Name} is not a class.");
            }

            // Ensure TRepositoryImpl implements TRepositoryInterface
            if (!repositoryInterfaceType.IsAssignableFrom(repositoryImplementationType))
            {
                throw new InvalidOperationException($"{repositoryImplementationType.Name} does not implement {repositoryInterfaceType.Name}.");
            }

            // Register TRepositoryInterface with the implementation TRepositoryImpl
            Services.AddScoped<TRepositoryInterface, TRepositoryImpl>();

            return this;
        }
    }
}
