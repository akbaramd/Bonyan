using Bonyan.Core;
using JetBrains.Annotations;

namespace Bonyan.UnitOfWork;

public class UnitOfWorkEventArgs : EventArgs
{
    /// <summary>
    /// Reference to the unit of work related to this event.
    /// </summary>
    public IBonUnitOfWork BonUnitOfWork { get; }

    public UnitOfWorkEventArgs([NotNull] IBonUnitOfWork bonUnitOfWork)
    {
        Check.NotNull(bonUnitOfWork, nameof(bonUnitOfWork));

        BonUnitOfWork = bonUnitOfWork;
    }
}
