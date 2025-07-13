using JetBrains.Annotations;

namespace Bonyan.UnitOfWork;

public interface IBonUnitOfWork : IDatabaseApiContainer, ITransactionApiContainer, IDisposable
{
    Guid Id { get; }

    Dictionary<string, object> Items { get; }

    //TODO: Switch to OnFailed (sync) and OnDisposed (sync) methods to be compatible with OnCompleted
    event EventHandler<UnitOfWorkFailedEventArgs> Failed;

    event EventHandler<UnitOfWorkEventArgs> Disposed;

    IBonUnitOfWorkOptions Options { get; }

    IBonUnitOfWork? Outer { get; }  

    bool IsReserved { get; }

    bool IsDisposed { get; }

    bool IsCompleted { get; }

    string? ReservationName { get; }

    void SetOuter(IBonUnitOfWork? outer);

    void Initialize([NotNull] BonUnitOfWorkOptions options);

    void Reserve([NotNull] string reservationName);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task CompleteAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);

    void OnCompleted(Func<Task> handler);

    void AddOrReplaceLocalEvent(
        UnitOfWorkEventRecord eventRecord,
        Predicate<UnitOfWorkEventRecord>? replacementSelector = null
    );

    void AddOrReplaceDistributedEvent(
        UnitOfWorkEventRecord eventRecord,
        Predicate<UnitOfWorkEventRecord>? replacementSelector = null
    );
}
