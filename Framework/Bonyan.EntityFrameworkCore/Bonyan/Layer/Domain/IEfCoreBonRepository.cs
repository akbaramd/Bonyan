using Bonyan.Layer.Domain.Entity;
using Bonyan.Layer.Domain.Repository.Abstractions;

namespace Bonyan.Layer.Domain
{
  // Readonly EfCore repository with both TEntity and TKey
  public interface IReadonlyEfCoreRepository<TEntity, in TKey> : IReadonlyEfCoreRepository<TEntity>, IBonReadOnlyRepository<TEntity, TKey> 
    where TEntity : class, IBonEntity<TKey> 
    where TKey : notnull
  {
    // Additional read-only methods specific to EF Core can be added here if needed
  }

  // Readonly EfCore repository with only TEntity
  public interface IReadonlyEfCoreRepository<TEntity> : IBonReadOnlyRepository<TEntity> 
    where TEntity : class, IBonEntity
  {
    // Additional read-only methods specific to EF Core can be added here if needed
  }
  
  // EfCore repository with both TEntity and TKey
  public interface IEfCoreBonRepository<TEntity, in TKey> : IEfCoreBonRepository<TEntity>, IBonRepository<TEntity, TKey> 
    where TEntity : class, IBonEntity<TKey> 
    where TKey : notnull
  {
    // Additional methods specific to EF Core can be added here if needed
  }

  // EfCore repository with only TEntity
  public interface IEfCoreBonRepository<TEntity> : IBonRepository<TEntity> 
    where TEntity : class, IBonEntity
  {
    // Additional methods specific to EF Core can be added here if needed
  }
}
