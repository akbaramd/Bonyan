using System.Collections.Immutable;
using Bonyan.Core;
using Bonyan.Exceptions;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace Bonyan.UnitOfWork;

public class BonUnitOfWork : IBonUnitOfWork
{
    /// <summary>
    /// Default: false.
    /// </summary>
    public static bool EnableObsoleteDbContextCreationWarning { get; } = false;

    public const string UnitOfWorkReservationName = "_BonyanActionUnitOfWork";

    public Guid Id { get; } = Guid.NewGuid();

    public IBonUnitOfWorkOptions Options { get; private set; } = default!;

    public IBonUnitOfWork? Outer { get; private set; }

    public bool IsReserved { get; set; }

    public bool IsDisposed { get; private set; }

    public bool IsCompleted { get; private set; }

    public string? ReservationName { get; set; }

    protected List<Func<Task>> CompletedHandlers { get; } = new List<Func<Task>>();
    protected List<UnitOfWorkEventRecord> DistributedEvents { get; } = new List<UnitOfWorkEventRecord>();
    protected List<UnitOfWorkEventRecord> LocalEvents { get; } = new List<UnitOfWorkEventRecord>();

    public event EventHandler<UnitOfWorkFailedEventArgs> Failed = default!;
    public event EventHandler<UnitOfWorkEventArgs> Disposed = default!;

    public IServiceProvider ServiceProvider { get; }
    protected IUnitOfWorkEventPublisher UnitOfWorkEventPublisher { get; }

    [NotNull]
    public Dictionary<string, object> Items { get; }

    private readonly Dictionary<string, IBonDatabaseApi> _databaseApis;
    private readonly Dictionary<string, IBonTransactionApi> _transactionApis;
    private readonly BonUnitOfWorkDefaultOptions _defaultOptions;

    private Exception? _exception;
    private bool _isCompleting;
    private bool _isRolledback;

    public BonUnitOfWork(
        IServiceProvider serviceProvider,
        IUnitOfWorkEventPublisher unitOfWorkEventPublisher,
        IOptions<BonUnitOfWorkDefaultOptions> options)
    {
        ServiceProvider = serviceProvider;
        UnitOfWorkEventPublisher = unitOfWorkEventPublisher;
        _defaultOptions = options.Value;

        _databaseApis = new Dictionary<string, IBonDatabaseApi>();
        _transactionApis = new Dictionary<string, IBonTransactionApi>();

        Items = new Dictionary<string, object>();
    }

    public virtual void Initialize(BonUnitOfWorkOptions options)
    {
        Check.NotNull(options, nameof(options));

        if (Options != null)
        {
            throw new BonException("This unit of work has already been initialized.");
        }

        Options = _defaultOptions.Normalize(options.Clone());
        IsReserved = false;
    }

    public virtual void Reserve(string reservationName)
    {
        Check.NotNullOrWhiteSpace(reservationName, nameof(reservationName));

        ReservationName = reservationName;
        IsReserved = true;
    }

    public virtual void SetOuter(IBonUnitOfWork? outer)
    {
        Outer = outer;
    }

    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_isRolledback)
        {
            return;
        }

        foreach (var databaseApi in GetAllActiveDatabaseApis())
        {
            if (databaseApi is ISupportsSavingChanges supportsSavingChangesDatabaseApi)
            {
                await supportsSavingChangesDatabaseApi.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public virtual IReadOnlyList<IBonDatabaseApi> GetAllActiveDatabaseApis()
    {
        return _databaseApis.Values.ToImmutableList();
    }

    public virtual IReadOnlyList<IBonTransactionApi> GetAllActiveTransactionApis()
    {
        return _transactionApis.Values.ToImmutableList();
    }

    public virtual async Task CompleteAsync(CancellationToken cancellationToken = default)
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

            while (LocalEvents.Any() || DistributedEvents.Any())
            {
                if (LocalEvents.Any())
                {
                    var localEventsToBePublished = LocalEvents.OrderBy(e => e.EventOrder).ToArray();
                    LocalEvents.Clear();
                    await UnitOfWorkEventPublisher.PublishLocalEventsAsync(
                        localEventsToBePublished
                    );
                }

                if (DistributedEvents.Any())
                {
                    var distributedEventsToBePublished = DistributedEvents.OrderBy(e => e.EventOrder).ToArray();
                    DistributedEvents.Clear();
                    await UnitOfWorkEventPublisher.PublishDistributedEventsAsync(
                        distributedEventsToBePublished
                    );
                }

                await SaveChangesAsync(cancellationToken);
            }

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

    public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_isRolledback)
        {
            return;
        }

        _isRolledback = true;

        await RollbackAllAsync(cancellationToken);
    }

    public virtual IBonDatabaseApi? FindDatabaseApi(string key)
    {
        return _databaseApis.GetOrDefault(key);
    }

    public virtual void AddDatabaseApi(string key, IBonDatabaseApi api)
    {
        Check.NotNullOrWhiteSpace(key, nameof(key));
        Check.NotNull(api, nameof(api));

        if (_databaseApis.ContainsKey(key))
        {
            throw new BonException("This unit of work already contains a database API for the given key.");
        }

        _databaseApis.Add(key, api);
    }

    public virtual IBonDatabaseApi GetOrAddDatabaseApi(string key, Func<IBonDatabaseApi> factory)
    {
        Check.NotNullOrWhiteSpace(key, nameof(key));
        Check.NotNull(factory, nameof(factory));

        return _databaseApis.GetOrAdd(key, factory);
    }

    public virtual IBonTransactionApi? FindTransactionApi(string key)
    {
        Check.NotNullOrWhiteSpace(key, nameof(key));

        return _transactionApis.GetOrDefault(key);
    }

    public virtual void AddTransactionApi(string key, IBonTransactionApi api)
    {
        Check.NotNullOrWhiteSpace(key, nameof(key));
        Check.NotNull(api, nameof(api));

        if (_transactionApis.ContainsKey(key))
        {
            throw new BonException("This unit of work already contains a transaction API for the given key.");
        }

        _transactionApis.Add(key, api);
    }

    public virtual IBonTransactionApi GetOrAddTransactionApi(string key, Func<IBonTransactionApi> factory)
    {
        Check.NotNullOrWhiteSpace(key, nameof(key));
        Check.NotNull(factory, nameof(factory));

        return _transactionApis.GetOrAdd(key, factory);
    }

    public virtual void OnCompleted(Func<Task> handler)
    {
        CompletedHandlers.Add(handler);
    }

    public virtual void AddOrReplaceLocalEvent(
        UnitOfWorkEventRecord eventRecord,
        Predicate<UnitOfWorkEventRecord>? replacementSelector = null)
    {
        AddOrReplaceEvent(LocalEvents, eventRecord, replacementSelector);
    }

    public virtual void AddOrReplaceDistributedEvent(
        UnitOfWorkEventRecord eventRecord,
        Predicate<UnitOfWorkEventRecord>? replacementSelector = null)
    {
        AddOrReplaceEvent(DistributedEvents, eventRecord, replacementSelector);
    }

    public virtual void AddOrReplaceEvent(
        List<UnitOfWorkEventRecord> eventRecords,
        UnitOfWorkEventRecord eventRecord,
        Predicate<UnitOfWorkEventRecord>? replacementSelector = null)
    {
        if (replacementSelector == null)
        {
            eventRecords.Add(eventRecord);
        }
        else
        {
            var foundIndex = eventRecords.FindIndex(replacementSelector);
            if (foundIndex < 0)
            {
                eventRecords.Add(eventRecord);
            }
            else
            {
                eventRecords[foundIndex] = eventRecord;
            }
        }
    }

    protected virtual async Task OnCompletedAsync()
    {
        foreach (var handler in CompletedHandlers)
        {
            await handler.Invoke();
        }
    }

    protected virtual void OnFailed()
    {
        Failed.InvokeSafely(this, new UnitOfWorkFailedEventArgs(this, _exception, _isRolledback));
    }

    protected virtual void OnDisposed()
    {
        Disposed.InvokeSafely(this, new UnitOfWorkEventArgs(this));
    }

    public virtual void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;

        DisposeTransactions();

        if (!IsCompleted || _exception != null)
        {
            OnFailed();
        }

        OnDisposed();
    }

    private void DisposeTransactions()
    {
        foreach (var transactionApi in GetAllActiveTransactionApis())
        {
            try
            {
                transactionApi.Dispose();
            }
            catch
            {
            }
        }
    }

    private void PreventMultipleComplete()
    {
        if (IsCompleted || _isCompleting)
        {
            throw new BonException("Completion has already been requested for this unit of work.");
        }
    }

    protected virtual async Task RollbackAllAsync(CancellationToken cancellationToken)
    {
        foreach (var databaseApi in GetAllActiveDatabaseApis())
        {
            if (databaseApi is ISupportsRollback supportsRollbackDatabaseApi)
            {
                try
                {
                    await supportsRollbackDatabaseApi.RollbackAsync(cancellationToken);
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
                    await supportsRollbackTransactionApi.RollbackAsync(cancellationToken);
                }
                catch { }
            }
        }
    }

    protected virtual async Task CommitTransactionsAsync(CancellationToken cancellationToken)
    {
        foreach (var transaction in GetAllActiveTransactionApis())
        {
            await transaction.CommitAsync(cancellationToken);
        }
    }

    public override string ToString()
    {
        return $"[UnitOfWork {Id}]";
    }
}
