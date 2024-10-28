using Bonyan.Layer.Domain.Abstractions;
using Bonyan.Layer.Domain.Entities;

namespace Bonyan.Layer.Domain
{
  // Readonly EfCore repository with both TEntity and TKey
  public interface IReadonlyEfCoreRepository<TEntity, in TKey> : IReadonlyEfCoreRepository<TEntity>, IReadOnlyRepository<TEntity, TKey> 
    where TEntity : class, IEntity<TKey> 
    where TKey : notnull
  {
    // Additional read-only methods specific to EF Core can be added here if needed
  }

  // Readonly EfCore repository with only TEntity
  public interface IReadonlyEfCoreRepository<TEntity> : IReadOnlyRepository<TEntity> 
    where TEntity : class, IEntity
  {
    // Additional read-only methods specific to EF Core can be added here if needed
  }
  
  // EfCore repository with both TEntity and TKey
  public interface IEfCoreRepository<TEntity, in TKey> : IEfCoreRepository<TEntity>, IRepository<TEntity, TKey> 
    where TEntity : class, IEntity<TKey> 
    where TKey : notnull
  {
    // Additional methods specific to EF Core can be added here if needed
  }

  // EfCore repository with only TEntity
  public interface IEfCoreRepository<TEntity> : IRepository<TEntity> 
    where TEntity : class, IEntity
  {
    // Additional methods specific to EF Core can be added here if needed
  }
}
