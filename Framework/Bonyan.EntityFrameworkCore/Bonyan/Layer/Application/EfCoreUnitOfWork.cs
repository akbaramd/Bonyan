using Bonyan.EntityFrameworkCore;
using Bonyan.Layer.Domain;
using Bonyan.Layer.Domain.Abstractions;
using Bonyan.Layer.Domain.Aggregates;
using Bonyan.Layer.Domain.Entities;
using Bonyan.Layer.Domain.Events;
using Microsoft.EntityFrameworkCore.Storage;

namespace Bonyan.Layer.Application;

public class EfCoreUnitOfWork<TDbContext> : IUnitOfWork where TDbContext : BonyanDbContext<TDbContext>
{
    private readonly TDbContext _dbContext;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private IDbContextTransaction? _transaction;

    public EfCoreUnitOfWork(TDbContext dbContext, IDomainEventDispatcher domainEventDispatcher)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _domainEventDispatcher = domainEventDispatcher ?? throw new ArgumentNullException(nameof(domainEventDispatcher));
    }

    public void BeginTransaction()
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _transaction = _dbContext.Database.BeginTransaction();
    }

    public async Task CommitAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction in progress to commit.");
        }

        try
        {
            await _dbContext.SaveChangesAsync();
            await DispatchDomainEventsAsync();
            await _transaction.CommitAsync();
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public void Rollback()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction in progress to rollback.");
        }

        _transaction.Rollback();
        DisposeTransactionAsync().GetAwaiter().GetResult();
    }

    private async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await DisposeTransactionAsync();
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        DisposeTransactionAsync().GetAwaiter().GetResult();
        GC.SuppressFinalize(this);
    }

    public bool IsTransactionActive()
    {
        return _transaction != null;
    }

    public void SaveChanges()
    {
        _dbContext.SaveChanges();
        DispatchDomainEventsAsync().GetAwaiter().GetResult();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
        await DispatchDomainEventsAsync();
    }

    public IReadOnlyRepository<TEntity> GetReadonlyRepository<TEntity>() where TEntity : class, IEntity
    {
        return new EfCoreReadonlyRepository<TEntity, TDbContext>(_dbContext);
    }

    public IReadOnlyRepository<TEntity, TKey> GetReadonlyRepository<TEntity, TKey>() where TEntity : class, IEntity<TKey> where TKey : notnull
    {
        return new EfCoreReadonlyRepository<TEntity, TKey, TDbContext>(_dbContext);
    }

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity
    {
        return new EfCoreRepository<TEntity, TDbContext>(_dbContext);
    }

    public IRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class, IEntity<TKey> where TKey : notnull
    {
        return new EfCoreRepository<TEntity, TKey, TDbContext>(_dbContext);
    }

    private async Task DispatchDomainEventsAsync()
    {
      // Retrieve all aggregates that implement IAggregateRoot with any type of key
      var aggregatesWithEvents = _dbContext.ChangeTracker
        .Entries()
        .Where(entry => entry.Entity is IAggregateRoot && ((IAggregateRoot)entry.Entity).DomainEvents.Any())
        .Select(entry => (IAggregateRoot)entry.Entity)
        .ToList();

      if (aggregatesWithEvents.Any())
      {
        // Dispatch events and clear them afterwards
        await _domainEventDispatcher.DispatchAndClearEvents(aggregatesWithEvents);
      }
    }

}
