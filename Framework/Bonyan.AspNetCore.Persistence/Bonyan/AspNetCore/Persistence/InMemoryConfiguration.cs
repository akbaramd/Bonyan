﻿using Bonyan.AspNetCore.Context;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Entities;
using Castle.DynamicProxy;

namespace Bonyan.AspNetCore.Persistence;

public class InMemoryConfiguration
{
  private readonly IBonyanContext Builder;
  private ProxyGenerator _proxyGenerator;

  public InMemoryConfiguration(IBonyanContext bonyanContext)
  {
    Builder = bonyanContext;
    _proxyGenerator = new ProxyGenerator();
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
      Builder.AddScoped(typeof(IRepository<,>).MakeGenericType(entityType, keyType), repositoryType);

      // Entity does not have a key, register the repository with InMemoryRepository<TEntity>
      repositoryType = typeof(InMemoryRepository<>).MakeGenericType(entityType);

      // Register the repository with IRepository<TEntity>
      Builder.AddScoped(typeof(IRepository<>).MakeGenericType(entityType), repositoryType);
    }
    else
    {
      // Entity does not have a key, register the repository with InMemoryRepository<TEntity>
      repositoryType = typeof(InMemoryRepository<>).MakeGenericType(entityType);

      // Register the repository with IRepository<TEntity>
      Builder.AddScoped(typeof(IRepository<>).MakeGenericType(entityType), repositoryType);
    }

    Builder.AddScoped<TRepository, TImplement>();
    Builder.AddScoped<TImplement>();

    return this;
  }

}
