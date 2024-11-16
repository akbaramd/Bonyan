namespace Bonyan.UnitOfWork;

public class BonAmbientBonUnitOfWork : IBonAmbientBonUnitOfWork
{
    public IBonUnitOfWork? UnitOfWork => _currentUow.Value;

    private readonly AsyncLocal<IBonUnitOfWork?> _currentUow;

    public BonAmbientBonUnitOfWork()
    {
        _currentUow = new AsyncLocal<IBonUnitOfWork?>();
    }

    public void SetUnitOfWork(IBonUnitOfWork? unitOfWork)
    {
        _currentUow.Value = unitOfWork;
    }

    public IBonUnitOfWork? GetCurrentByChecking()
    {
        var uow = UnitOfWork;

        //Skip reserved unit of work
        while (uow != null && (uow.IsReserved || uow.IsDisposed || uow.IsCompleted))
        {
            uow = uow.Outer;
        }

        return uow;
    }
}
