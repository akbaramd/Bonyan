using System.Collections.Immutable;
using Bonyan.EntityFrameworkCore;
using Bonyan.Layer.Domain;
using Bonyan.Layer.Domain.Abstractions;
using Bonyan.Layer.Domain.Aggregates;
using Bonyan.Layer.Domain.Entities;
using Bonyan.Layer.Domain.Events;
using Bonyan.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.Uow;

namespace Bonyan.Layer.Application;

public class EfCoreUnitOfWork : IUnitOfWork
{
  private readonly IDomainEventDispatcher _domainEventDispatcher;
  private IDbContextTransaction? _transaction;
  public IServiceProvider ServiceProvider { get; } = default!;
  private Exception? _exception;
  private bool _isCompleting = false;
  private bool _isRolledback = false;
  private readonly Dictionary<string, IDatabaseApi> _databaseApis;
  private readonly Dictionary<string, ITransactionApi> _transactionApis;
  protected List<Func<Task>> CompletedHandlers { get; } = new List<Func<Task>>();
  public bool IsReserved { get; set; }

  public bool IsDisposed { get; private set; }

  public bool IsCompleted { get; private set; }

  public string? ReservationName { get; set; }
  public EfCoreUnitOfWork(IDomainEventDispatcher domainEventDispatcher)
  {
    _domainEventDispatcher = domainEventDispatcher ?? throw new ArgumentNullException(nameof(domainEventDispatcher));
    _databaseApis = new Dictionary<string, IDatabaseApi>();
    _transactionApis = new Dictionary<string, ITransactionApi>();
  }


  public async Task CommitAsync(CancellationToken? cancellationToken = default)
  {
    if (_isRolledback)
    {
      return;
    }

    PreventMultipleComplete();
    
    try
    {
      _isCompleting = true;
      await SaveChangesAsync(cancellationToken);

      await CommitTransactionsAsync(cancellationToken);
      IsCompleted = true;
      await OnCompletedAsync();
    }
    catch (Exception ex)
    {
      _exception = ex;
      throw;
    }

    
  }



  private void PreventMultipleComplete()
  {
    if (IsCompleted || _isCompleting)
    {
      throw new Exception("Completion has already been requested for this unit of work.");
    }
  }


  public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
  {
    if (_isRolledback)
    {
      return;
    }

    _isRolledback = true;

    await RollbackAllAsync(cancellationToken);
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

 

  public async Task SaveChangesAsync(CancellationToken? cancellationToken = default)
  {
    if (_isRolledback)
    {
      return;
    }

    foreach (var databaseApi in GetAllActiveDatabaseApis())
    {
      if (databaseApi is ISupportsSavingChanges supportsSavingChangesDatabaseApi)
      {
        await supportsSavingChangesDatabaseApi.SaveChangesAsync(cancellationToken ?? CancellationToken.None);
      }
    }

    await DispatchDomainEventsAsync();
  }

  public virtual IReadOnlyList<IDatabaseApi> GetAllActiveDatabaseApis()
  {
    return _databaseApis.Values.ToImmutableList();
  }

  public virtual IReadOnlyList<ITransactionApi> GetAllActiveTransactionApis()
  {
    return _transactionApis.Values.ToImmutableList();
  }

  public virtual IDatabaseApi? FindDatabaseApi(string key)
  {
    return _databaseApis.GetOrDefault(key);
  }

  private async Task DispatchDomainEventsAsync()
  {
    foreach (var databaseApi in GetAllActiveDatabaseApis())
    {
      if (databaseApi is IEfCoreDbContext dbContext)
      {
        var aggregatesWithEvents = dbContext.ChangeTracker
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
  }

  public virtual void AddDatabaseApi(string key, IDatabaseApi api)
  {
    Check.NotNull(key, nameof(key));
    Check.NotNull(api, nameof(api));

    if (_databaseApis.ContainsKey(key))
    {
      throw new Exception("This unit of work already contains a database API for the given key.");
    }

    _databaseApis.Add(key, api);
  }

  public virtual IDatabaseApi GetOrAddDatabaseApi(string key, Func<IDatabaseApi> factory)
  {
    Check.NotNull(key, nameof(key));
    Check.NotNull(factory, nameof(factory));

    return _databaseApis.GetOrAdd(key, factory);
  }

  public virtual ITransactionApi? FindTransactionApi(string key)
  {
    Check.NotNull(key, nameof(key));

    return _transactionApis.GetOrDefault(key);
  }

  public virtual void AddTransactionApi(string key, ITransactionApi api)
  {
    Check.NotNull(key, nameof(key));
    Check.NotNull(api, nameof(api));

    if (_transactionApis.ContainsKey(key))
    {
      throw new Exception("This unit of work already contains a transaction API for the given key.");
    }

    _transactionApis.Add(key, api);
  }

  public virtual ITransactionApi GetOrAddTransactionApi(string key, Func<ITransactionApi> factory)
  {
    Check.NotNull(key, nameof(key));
    Check.NotNull(factory, nameof(factory));

    return _transactionApis.GetOrAdd(key, factory);
  }

  
  protected virtual async Task RollbackAllAsync(CancellationToken? cancellationToken)
  {
    foreach (var databaseApi in GetAllActiveDatabaseApis())
    {
      if (databaseApi is ISupportsRollback supportsRollbackDatabaseApi)
      {
        try
        {
          await supportsRollbackDatabaseApi.RollbackAsync(cancellationToken??CancellationToken.None);
        }
        catch { }
      }
    }

    foreach (var transactionApi in GetAllActiveTransactionApis())
    {
      if (transactionApi is ISupportsRollback supportsRollbackTransactionApi)
      {
        try
        {
          await supportsRollbackTransactionApi.RollbackAsync(cancellationToken??CancellationToken.None);
        }
        catch { }
      }
    }
  }

  protected virtual async Task CommitTransactionsAsync(CancellationToken? cancellationToken)
  {
    foreach (var transaction in GetAllActiveTransactionApis())
    {
      await transaction.CommitAsync(cancellationToken??CancellationToken.None);
    }
  }
  protected virtual async Task OnCompletedAsync()
  {
    foreach (var handler in CompletedHandlers)
    {
      await handler.Invoke();
    }
  }
  public virtual void OnCompleted(Func<Task> handler)
  {
    CompletedHandlers.Add(handler);
  }
}
