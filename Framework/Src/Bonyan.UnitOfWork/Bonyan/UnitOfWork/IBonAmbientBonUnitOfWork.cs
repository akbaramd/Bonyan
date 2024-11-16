namespace Bonyan.UnitOfWork;

public interface IBonAmbientBonUnitOfWork : IBonUnitOfWorkAccessor
{
    IBonUnitOfWork? GetCurrentByChecking();
}
