using JetBrains.Annotations;

namespace Bonyan.UnitOfWork;

/// <summary>
/// Used as event arguments on <see cref="IBonUnitOfWork.Failed"/> event.
/// </summary>
public class UnitOfWorkFailedEventArgs : UnitOfWorkEventArgs
{
    /// <summary>
    /// Exception that caused failure. This is set only if an error occurred during <see cref="IBonUnitOfWork.CompleteAsync"/>.
    /// Can be null if there is no exception, but <see cref="IBonUnitOfWork.CompleteAsync"/> is not called.
    /// Can be null if another exception occurred during the UOW.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// True, if the unit of work is manually rolled back.
    /// </summary>
    public bool IsRolledback { get; }

    /// <summary>
    /// Creates a new <see cref="UnitOfWorkFailedEventArgs"/> object.
    /// </summary>
    public UnitOfWorkFailedEventArgs([NotNull] IBonUnitOfWork bonUnitOfWork, Exception? exception, bool isRolledback)
        : base(bonUnitOfWork)
    {
        Exception = exception;
        IsRolledback = isRolledback;
    }
}
