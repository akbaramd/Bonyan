using System.Data;

namespace Bonyan.UnitOfWork;

public class BonyanUnitOfWorkOptions : IBonyanUnitOfWorkOptions
{
    /// <summary>
    /// Default: false.
    /// </summary>
    public bool IsTransactional { get; set; }

    public IsolationLevel? IsolationLevel { get; set; }

    /// <summary>
    /// Milliseconds
    /// </summary>
    public int? Timeout { get; set; }

    public BonyanUnitOfWorkOptions()
    {

    }

    public BonyanUnitOfWorkOptions(bool isTransactional = false, IsolationLevel? isolationLevel = null, int? timeout = null)
    {
        IsTransactional = isTransactional;
        IsolationLevel = isolationLevel;
        Timeout = timeout;
    }

    public BonyanUnitOfWorkOptions Clone()
    {
        return new BonyanUnitOfWorkOptions
        {
            IsTransactional = IsTransactional,
            IsolationLevel = IsolationLevel,
            Timeout = Timeout
        };
    }
}
