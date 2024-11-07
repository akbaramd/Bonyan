using Bonyan.Helpers;
using Bonyan.Layer.Domain.Abstractions;
using Bonyan.Layer.Domain.Entities;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionRepositoryExtensions
{
    public static IServiceCollection AddDefaultRepository(
        this IServiceCollection services,
        Type entityType,
        Type repositoryImplementationType,
        bool replaceExisting = false)
    {
        //IReadOnlyBasicRepository<TEntity>
        var readOnlyBasicRepositoryInterface = typeof(IBonReadOnlyRepository<>).MakeGenericType(entityType);
        if (readOnlyBasicRepositoryInterface.IsAssignableFrom(repositoryImplementationType))
        {
            RegisterService(services, readOnlyBasicRepositoryInterface, repositoryImplementationType, replaceExisting, true);

            //IReadOnlyRepository<TEntity>
            var readOnlyRepositoryInterface = typeof(IBonReadOnlyRepository<>).MakeGenericType(entityType);
            if (readOnlyRepositoryInterface.IsAssignableFrom(repositoryImplementationType))
            {
                RegisterService(services, readOnlyRepositoryInterface, repositoryImplementationType, replaceExisting, true);
            }

            //IBasicRepository<TEntity>
            var basicRepositoryInterface = typeof(IBonRepository<>).MakeGenericType(entityType);
            if (basicRepositoryInterface.IsAssignableFrom(repositoryImplementationType))
            {
                RegisterService(services, basicRepositoryInterface, repositoryImplementationType, replaceExisting);

                //IRepository<TEntity>
                var repositoryInterface = typeof(IBonRepository<>).MakeGenericType(entityType);
                if (repositoryInterface.IsAssignableFrom(repositoryImplementationType))
                {
                    RegisterService(services, repositoryInterface, repositoryImplementationType, replaceExisting);
                }
            }
        }

        var primaryKeyType = BonEntityHelper.FindPrimaryKeyType(entityType);
        if (primaryKeyType != null)
        {
            //IReadOnlyBasicRepository<TEntity, TKey>
            var readOnlyBasicRepositoryInterfaceWithPk = typeof(IBonReadOnlyRepository<,>).MakeGenericType(entityType, primaryKeyType);
            if (readOnlyBasicRepositoryInterfaceWithPk.IsAssignableFrom(repositoryImplementationType))
            {
                RegisterService(services, readOnlyBasicRepositoryInterfaceWithPk, repositoryImplementationType, replaceExisting, true);

                //IReadOnlyRepository<TEntity, TKey>
                var readOnlyRepositoryInterfaceWithPk = typeof(IBonReadOnlyRepository<,>).MakeGenericType(entityType, primaryKeyType);
                if (readOnlyRepositoryInterfaceWithPk.IsAssignableFrom(repositoryImplementationType))
                {
                    RegisterService(services, readOnlyRepositoryInterfaceWithPk, repositoryImplementationType, replaceExisting, true);
                }

                //IBasicRepository<TEntity, TKey>
                var basicRepositoryInterfaceWithPk = typeof(IBonRepository<,>).MakeGenericType(entityType, primaryKeyType);
                if (basicRepositoryInterfaceWithPk.IsAssignableFrom(repositoryImplementationType))
                {
                    RegisterService(services, basicRepositoryInterfaceWithPk, repositoryImplementationType, replaceExisting);

                    //IRepository<TEntity, TKey>
                    var repositoryInterfaceWithPk = typeof(IBonRepository<,>).MakeGenericType(entityType, primaryKeyType);
                    if (repositoryInterfaceWithPk.IsAssignableFrom(repositoryImplementationType))
                    {
                        RegisterService(services, repositoryInterfaceWithPk, repositoryImplementationType, replaceExisting);
                    }
                }
            }
        }

        return services;
    }

    private static void RegisterService(
        IServiceCollection services,
        Type serviceType,
        Type implementationType,
        bool replaceExisting,
        bool isReadOnlyRepository = false)
    {
        ServiceDescriptor descriptor;

        if (isReadOnlyRepository)
        {
            services.TryAddTransient(implementationType);
            descriptor = ServiceDescriptor.Transient(serviceType, provider =>
            {
                var repository = provider.GetRequiredService(implementationType);
                ObjectHelper.TrySetProperty(repository.As<IBonRepository>(), x => x.IsChangeTrackingEnabled, _ => false);
                return repository;
            });
        }
        else
        {
            descriptor = ServiceDescriptor.Transient(serviceType, implementationType);
        }

        if (replaceExisting)
        {
            services.Replace(descriptor);
        }
        else
        {
            services.TryAdd(descriptor);
        }
    }
    
    
    
}
