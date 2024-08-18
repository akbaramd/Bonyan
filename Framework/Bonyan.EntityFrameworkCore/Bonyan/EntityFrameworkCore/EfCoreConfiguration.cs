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

namespace Bonyan.EntityFrameworkCore;

public class EfCoreConfiguration
{
  public EfCoreConfiguration(IServiceCollection services)
  {
    Services = services;
  }

  public IServiceCollection Services { get; }
  public Action<DbContextOptionsBuilder> DbContextOptionsAction { get; set; } = _ => { };

  public EfCoreConfiguration AddDbContext<TContext>(Action<DbContextOptionsBuilder> optionsAction)
    where TContext : DbContext
  {
    DbContextOptionsAction = optionsAction;
    Services.AddDbContext<TContext>(optionsAction);
    // Register repositories and UnitOfWork
    RegisterRepositories<TContext>();
    return this;
  }

  private void RegisterRepositories<TContext>() where TContext : DbContext
  {
    var dbContextType = typeof(TContext);
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

        // Register IRepository<TEntity, TKey> with EfCoreRepository<TEntity, TKey, TContext>
        var repositoryWithKeyType = typeof(EfCoreRepository<,,>).MakeGenericType(type, keyType, dbContextType);
        var repositoryWithKeyInterfaceType = typeof(IRepository<,>).MakeGenericType(type, keyType);
        Services.AddScoped(repositoryWithKeyInterfaceType, repositoryWithKeyType);
      }
      else
      {
        // Register IRepository<TEntity> with EfCoreRepository<TEntity, TContext>
        var repositoryType = typeof(EfCoreRepository<,>).MakeGenericType(type, dbContextType);
        var repositoryInterfaceType = typeof(IRepository<>).MakeGenericType(type);
        Services.AddScoped(repositoryInterfaceType, repositoryType);
      }
    }

    // Register IUnitOfWork with EfCoreUnitOfWork<TContext>
    Services.AddScoped(typeof(IUnitOfWork), typeof(EfCoreUnitOfWork<TContext>));
  }


  public EfCoreConfiguration EnsureDatabaseCreated<TContext>()
    where TContext : DbContext
  {
    Services.AddScoped<IHostedService, EnsureDatabaseCreatedService<TContext>>();
    return this;
  }

  public EfCoreConfiguration ApplyMigrations<TContext>()
    where TContext : DbContext
  {
    Services.AddScoped<IHostedService, ApplyMigrationsService<TContext>>();
    return this;
  }


  public EfCoreConfiguration AddSeed<TSeed>() where TSeed : SeedBase
  {
    Services.AddTransient<SeedBase, TSeed>();
    return this;
  }

  public EfCoreConfiguration ConfigureLogging<TContext>(Action<ILoggingBuilder> configureLogging)
    where TContext : DbContext
  {
    Services.AddDbContext<TContext>((serviceProvider, options) =>
    {
      configureLogging(serviceProvider.GetRequiredService<ILoggingBuilder>());
    });

    return this;
  }
}
