using JetBrains.Annotations;

namespace Bonyan.UnitOfWork;

public interface IBonUnitOfWorkManager
{
    IBonUnitOfWork? Current { get; }

    [NotNull]
    IBonUnitOfWork Begin([NotNull] BonUnitOfWorkOptions options, bool requiresNew = false);

    [NotNull]
    IBonUnitOfWork Reserve([NotNull] string reservationName, bool requiresNew = false);

    void BeginReserved([NotNull] string reservationName, [NotNull] BonUnitOfWorkOptions options);

    bool TryBeginReserved([NotNull] string reservationName, [NotNull] BonUnitOfWorkOptions options);
}
