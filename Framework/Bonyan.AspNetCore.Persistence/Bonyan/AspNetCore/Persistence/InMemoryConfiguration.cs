using Bonyan.DDD.Domain.Abstractions;
using Bonyan.DDD.Domain.Entities;
using Castle.DynamicProxy;

namespace Bonyan.AspNetCore.Persistence;

public class InMemoryConfiguration
{
  private readonly IBonyanApplicationBuilder Builder;
  private ProxyGenerator _proxyGenerator;

  public InMemoryConfiguration(IBonyanApplicationBuilder builderServices)
  {
    Builder = builderServices;
    _proxyGenerator = new ProxyGenerator();
  }
  
  
  public InMemoryConfiguration AddRepository<TRepository>() where TRepository : class, IRepository
  {
    // Find the entity type from the TRepository
    var repositoryInterface = typeof(TRepository).GetInterfaces()
      .FirstOrDefault(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(IRepository<>)
                                               || i.GetGenericTypeDefinition() == typeof(IRepository<,>)));

    if (repositoryInterface == null)
    {
      throw new InvalidOperationException(
        "TRepository does not implement IRepository<TEntity> or IRepository<TEntity, TKey>.");
    }

    var entityType = repositoryInterface.GetGenericArguments()[0];

    // Check if the entity implements IEntity<TKey>
    var keyInterface = entityType.GetInterfaces()
      .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>));

    Type repositoryType;
    if (keyInterface != null)
    {
      // Entity has a key, get TKey and register the repository with InMemoryRepository<TEntity, TKey>
      var keyType = keyInterface.GetGenericArguments()[0];
      repositoryType = typeof(InMemoryRepository<,>).MakeGenericType(entityType, keyType);

      // Register the repository with IRepository<TEntity, TKey>
      Builder.Services.AddScoped(typeof(IRepository<,>).MakeGenericType(entityType, keyType), repositoryType);

      // Entity does not have a key, register the repository with InMemoryRepository<TEntity>
      repositoryType = typeof(InMemoryRepository<>).MakeGenericType(entityType);

      // Register the repository with IRepository<TEntity>
      Builder.Services.AddScoped(typeof(IRepository<>).MakeGenericType(entityType), repositoryType);
    }
    else
    {
      // Entity does not have a key, register the repository with InMemoryRepository<TEntity>
      repositoryType = typeof(InMemoryRepository<>).MakeGenericType(entityType);

      // Register the repository with IRepository<TEntity>
      Builder.Services.AddScoped(typeof(IRepository<>).MakeGenericType(entityType), repositoryType);
    }

    // Create the proxy for the repository
    var proxyType = CreateProxy(repositoryType, typeof(TRepository));

    // Register the proxy repository type as the implementation of TRepository
    Builder.Services.AddScoped(typeof(TRepository), proxyType);

    return this;
  }

  // Method to create a dynamic proxy class for the repository
  private Type CreateProxy(Type baseRepositoryType, Type repositoryInterface)
  {
    var proxyOptions = new ProxyGenerationOptions();

    // Here, you can add custom interceptors to handle logging, transactions, etc.
    var proxy = _proxyGenerator.CreateClassProxy(baseRepositoryType, new[] { repositoryInterface }, proxyOptions,
      new RepositoryInterceptor());

    return proxy.GetType();
  }

  // Add repository with interface and implementation type
  public InMemoryConfiguration AddRepository<TRepository, TImplement>()
    where TRepository : class, IRepository
    where TImplement : class, TRepository
  {
    AddRepository<TRepository>();

    Builder.Services.AddScoped<TRepository, TImplement>();
    Builder.Services.AddScoped<TImplement>();

    return this;
  }

}
