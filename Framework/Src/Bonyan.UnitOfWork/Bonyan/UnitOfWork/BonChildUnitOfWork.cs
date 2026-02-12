using Bonyan.Core;
using JetBrains.Annotations;

namespace Bonyan.UnitOfWork;

/// <summary>
/// Lightweight handle to an existing (parent) unit of work. Delegates all operations to the parent.
/// Does not own the parent: <see cref="Dispose"/> is a no-op and <see cref="CompleteAsync"/> does not complete the parent.
/// Used when <see cref="IBonUnitOfWorkManager.Begin"/> or <see cref="IBonUnitOfWorkManager.Reserve"/> is called
/// while a current UoW already exists and <c>requiresNew</c> is false.
/// </summary>
internal class BonChildUnitOfWork : IBonUnitOfWork
{
    public Guid Id => _parent.Id;

    public IBonUnitOfWorkOptions Options => _parent.Options;

    public IBonUnitOfWork? Outer => _parent.Outer;

    public bool IsReserved => _parent.IsReserved;

    public bool IsDisposed => _parent.IsDisposed;

    public bool IsCompleted => _parent.IsCompleted;

    public string? ReservationName => _parent.ReservationName;

    public event EventHandler<UnitOfWorkFailedEventArgs> Failed = default!;
    public event EventHandler<UnitOfWorkEventArgs> Disposed = default!;

    public IServiceProvider ServiceProvider => _parent.ServiceProvider;

    public Dictionary<string, object> Items => _parent.Items;

    private readonly IBonUnitOfWork _parent;

    public BonChildUnitOfWork([NotNull] IBonUnitOfWork parent)
    {
        Check.NotNull(parent, nameof(parent));

        _parent = parent;

        _parent.Failed += (sender, args) => { Failed.InvokeSafely(sender!, args); };
        _parent.Disposed += (sender, args) => { Disposed.InvokeSafely(sender!, args); };
    }

    public void SetOuter(IBonUnitOfWork? outer)
    {
        _parent.SetOuter(outer);
    }

    public void Initialize(BonUnitOfWorkOptions options)
    {
        _parent.Initialize(options);
    }

    public void Reserve(string reservationName)
    {
        _parent.Reserve(reservationName);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _parent.SaveChangesAsync(cancellationToken); 
    }

    public Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        return _parent.RollbackAsync(cancellationToken);
    }

    public void OnCompleted(Func<Task> handler)
    {
        _parent.OnCompleted(handler);
    }

    public void AddOrReplaceLocalEvent(
        UnitOfWorkEventRecord eventRecord,
        Predicate<UnitOfWorkEventRecord>? replacementSelector = null)
    {
        _parent.AddOrReplaceLocalEvent(eventRecord, replacementSelector);
    }

    public void AddOrReplaceDistributedEvent(
        UnitOfWorkEventRecord eventRecord,
        Predicate<UnitOfWorkEventRecord>? replacementSelector = null)
    {
        _parent.AddOrReplaceDistributedEvent(eventRecord, replacementSelector);
    }

    public IBonDatabaseApi? FindDatabaseApi(string key)
    {
        return _parent.FindDatabaseApi(key);
    }

    public void AddDatabaseApi(string key, IBonDatabaseApi api)
    {
        _parent.AddDatabaseApi(key, api);
    }

    public IBonDatabaseApi GetOrAddDatabaseApi(string key, Func<IBonDatabaseApi> factory)
    {
        return _parent.GetOrAddDatabaseApi(key, factory);
    }

  

    public IBonTransactionApi? FindTransactionApi(string key)
    {
        return _parent.FindTransactionApi(key);
    }

    public void AddTransactionApi(string key, IBonTransactionApi api)
    {
        _parent.AddTransactionApi(key, api);
    }

    public IBonTransactionApi GetOrAddTransactionApi(string key, Func<IBonTransactionApi> factory)
    {
        return _parent.GetOrAddTransactionApi(key, factory);
    }

    /// <summary>
    /// No-op: the child does not own the parent; only the scope that created the real UoW disposes it.
    /// </summary>
    public void Dispose()
    {
        // Child is a handle only; parent is disposed by BonUnitOfWorkManager when the creating scope ends.
    }

    public override string ToString()
    {
        return $"[UnitOfWork {Id}]";
    }
}
