using System.Data;

namespace Bonyan.UnitOfWork;

public interface IBonyanUnitOfWorkOptions
{
    bool IsTransactional { get; }

    IsolationLevel? IsolationLevel { get; }

    /// <summary>
    /// Milliseconds
    /// </summary>
    int? Timeout { get; }
}
