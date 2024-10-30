namespace Bonyan.UnitOfWork;

public interface IAmbientUnitOfWork : IUnitOfWorkAccessor
{
    IUnitOfWork? GetCurrentByChecking();
}
