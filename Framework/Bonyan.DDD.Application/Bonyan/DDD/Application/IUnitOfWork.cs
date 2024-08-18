using Bonyan.DDD.Domain.Abstractions;
using Bonyan.DDD.Domain.Entities;

namespace Bonyan.DDD.Application;

public interface IUnitOfWork : IDisposable
{
  // Begin a transaction
  void BeginTransaction();

  // Commit the transaction asynchronously
  Task CommitAsync();

  // Rollback the transaction
  void Rollback();

  // Check if a transaction is currently active
  bool IsTransactionActive();

  // Save changes synchronously
  void SaveChanges();

  // Save changes asynchronously
  Task SaveChangesAsync();

  // Get a repository for a specific entity type
  IRepository<TEntity> GetRepository<TEntity>()  where TEntity : class, IEntity;
  IRepository<TEntity,TKey> GetRepository<TEntity,TKey>()  where TEntity : class, IEntity;
}
