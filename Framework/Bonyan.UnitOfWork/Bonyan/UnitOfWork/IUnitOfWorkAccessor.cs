namespace Bonyan.UnitOfWork;

public interface IUnitOfWorkAccessor
{
    IUnitOfWork? UnitOfWork { get; }

    void SetUnitOfWork(IUnitOfWork? unitOfWork);
}
