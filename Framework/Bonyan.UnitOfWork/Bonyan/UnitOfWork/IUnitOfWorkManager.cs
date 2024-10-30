using JetBrains.Annotations;

namespace Bonyan.UnitOfWork;

public interface IUnitOfWorkManager
{
    IUnitOfWork? Current { get; }

    [NotNull]
    IUnitOfWork Begin([NotNull] BonyanUnitOfWorkOptions options, bool requiresNew = false);

    [NotNull]
    IUnitOfWork Reserve([NotNull] string reservationName, bool requiresNew = false);

    void BeginReserved([NotNull] string reservationName, [NotNull] BonyanUnitOfWorkOptions options);

    bool TryBeginReserved([NotNull] string reservationName, [NotNull] BonyanUnitOfWorkOptions options);
}
