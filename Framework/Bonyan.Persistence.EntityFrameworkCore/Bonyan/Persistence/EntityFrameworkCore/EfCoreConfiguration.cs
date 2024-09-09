using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Entities;
using Bonyan.Persistence.EntityFrameworkCore.HostedServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bonyan.Persistence.EntityFrameworkCore
{
  public class EfCoreConfiguration<TDbContext> where TDbContext : DbContext
  {
    public EfCoreConfiguration(IServiceCollection services)
    {
      Services = services;

    }

    public IServiceCollection Services { get; }
    public Action<DbContextOptionsBuilder> DbContextOptionsAction { get; set; } = _ => { };


    public EfCoreConfiguration<TDbContext> Configure(Action<DbContextOptionsBuilder> action)
    {
      DbContextOptionsAction = action;
      return this;
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

    // Add repository with a specific implementation
    public EfCoreConfiguration<TDbContext> AddRepository<TRepository, TImplement>()
      where TRepository : class, IRepository
      where TImplement : class, TRepository
    {
      // Find the repository interface for the entity
      var repositoryInterface = typeof(TRepository).GetInterfaces()
        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>)
                             || i.GetGenericTypeDefinition() == typeof(IRepository<,>));

      if (repositoryInterface == null)
      {
        throw new InvalidOperationException("TRepository does not implement IRepository<TEntity> or IRepository<TEntity, TKey>.");
      }

      var entityType = repositoryInterface.GetGenericArguments()[0]; // Get TEntity

      // Check if the entity implements IEntity<TKey>
      var keyInterface = entityType.GetInterfaces()
        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>));

      if (keyInterface != null)
      {
        // Entity has a key (IEntity<TKey>), extract TKey
        var keyType = keyInterface.GetGenericArguments()[0];

        // Register the repository with IEntity and TKey
        var repositoryTypeWithKey = typeof(IRepository<,>).MakeGenericType(entityType, keyType);
        var implementationTypeWithKey = typeof(TImplement);

        // Register the repository service with TEntity and TKey
        Services.AddScoped(repositoryTypeWithKey, implementationTypeWithKey);
        
        // Entity does not have a key, register IRepository<TEntity>
        var repositoryType = typeof(IRepository<>).MakeGenericType(entityType);
        var implementationType = typeof(TImplement);

        // Register the repository service with TEntity
        Services.AddScoped(repositoryType, implementationType);
      }
      else
      {
        // Entity does not have a key, register IRepository<TEntity>
        var repositoryType = typeof(IRepository<>).MakeGenericType(entityType);
        var implementationType = typeof(TImplement);

        // Register the repository service with TEntity
        Services.AddScoped(repositoryType, implementationType);
      }

      // Also register the TRepository with TImplement for general use
      Services.AddScoped<TRepository, TImplement>();
      Services.AddScoped<TImplement>();

      return this;
    }


  }
}

