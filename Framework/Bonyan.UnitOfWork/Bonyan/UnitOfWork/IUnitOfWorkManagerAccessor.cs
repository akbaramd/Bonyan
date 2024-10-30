namespace Bonyan.UnitOfWork;

public interface IUnitOfWorkManagerAccessor
{
    IUnitOfWorkManager UnitOfWorkManager { get; }
}
