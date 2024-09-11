using Bonyan.DDD.Domain;
using Bonyan.DomainDrivenDesign.Application;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Bonyan.DDD.Application;

public class EfCoreUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _dbContext;
    private readonly IServiceProvider _serviceProvider;
    private IDbContextTransaction? _transaction;

    public EfCoreUnitOfWork(TContext dbContext, IServiceProvider serviceProvider)
    {
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      _serviceProvider = serviceProvider;
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
        throw new NotImplementedException();
    }

    public void SaveChanges()
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity
    {
        return new EfCoreRepository<TEntity, TContext>(_dbContext,_serviceProvider);
    }

     public IRepository<TEntity,TKey> GetRepository<TEntity,TKey>() where TEntity : class, IEntity
    {
        return new EfCoreRepository<TEntity,TKey, TContext>(_dbContext,_serviceProvider);
    }
}