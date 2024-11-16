namespace Bonyan.UnitOfWork;

public interface IBonUnitOfWorkAccessor
{
    IBonUnitOfWork? UnitOfWork { get; }

    void SetUnitOfWork(IBonUnitOfWork? unitOfWork);
}
