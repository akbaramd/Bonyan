using System.Data;
using Bonyan.Core;
using JetBrains.Annotations;

namespace Bonyan.UnitOfWork;

public static class UnitOfWorkManagerExtensions
{
    [NotNull]
    public static IBonUnitOfWork Begin(
        [NotNull] this IBonUnitOfWorkManager bonUnitOfWorkManager,
        bool requiresNew = false,
        bool isTransactional = false,
        IsolationLevel? isolationLevel = null,
        int? timeout = null)
    {
        Check.NotNull(bonUnitOfWorkManager, nameof(bonUnitOfWorkManager));

        return bonUnitOfWorkManager.Begin(new BonUnitOfWorkOptions
        {
            IsTransactional = isTransactional,
            IsolationLevel = isolationLevel,
            Timeout = timeout
        }, requiresNew);
    }

    public static void BeginReserved([NotNull] this IBonUnitOfWorkManager bonUnitOfWorkManager, [NotNull] string reservationName)
    {
        Check.NotNull(bonUnitOfWorkManager, nameof(bonUnitOfWorkManager));
        Check.NotNull(reservationName, nameof(reservationName));

        bonUnitOfWorkManager.BeginReserved(reservationName, new BonUnitOfWorkOptions());
    }

    public static void TryBeginReserved([NotNull] this IBonUnitOfWorkManager bonUnitOfWorkManager, [NotNull] string reservationName)
    {
        Check.NotNull(bonUnitOfWorkManager, nameof(bonUnitOfWorkManager));
        Check.NotNull(reservationName, nameof(reservationName));

        bonUnitOfWorkManager.TryBeginReserved(reservationName, new BonUnitOfWorkOptions());
    }
}
