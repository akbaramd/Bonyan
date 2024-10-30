using System.Data;
using Bonyan.Core;
using JetBrains.Annotations;

namespace Bonyan.UnitOfWork;

public static class UnitOfWorkManagerExtensions
{
    [NotNull]
    public static IUnitOfWork Begin(
        [NotNull] this IUnitOfWorkManager unitOfWorkManager,
        bool requiresNew = false,
        bool isTransactional = false,
        IsolationLevel? isolationLevel = null,
        int? timeout = null)
    {
        Check.NotNull(unitOfWorkManager, nameof(unitOfWorkManager));

        return unitOfWorkManager.Begin(new BonyanUnitOfWorkOptions
        {
            IsTransactional = isTransactional,
            IsolationLevel = isolationLevel,
            Timeout = timeout
        }, requiresNew);
    }

    public static void BeginReserved([NotNull] this IUnitOfWorkManager unitOfWorkManager, [NotNull] string reservationName)
    {
        Check.NotNull(unitOfWorkManager, nameof(unitOfWorkManager));
        Check.NotNull(reservationName, nameof(reservationName));

        unitOfWorkManager.BeginReserved(reservationName, new BonyanUnitOfWorkOptions());
    }

    public static void TryBeginReserved([NotNull] this IUnitOfWorkManager unitOfWorkManager, [NotNull] string reservationName)
    {
        Check.NotNull(unitOfWorkManager, nameof(unitOfWorkManager));
        Check.NotNull(reservationName, nameof(reservationName));

        unitOfWorkManager.TryBeginReserved(reservationName, new BonyanUnitOfWorkOptions());
    }
}
