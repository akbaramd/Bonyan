namespace Bonyan.UnitOfWork;


public class AlwaysDisableTransactionsBonUnitOfWorkManager : IBonUnitOfWorkManager
{
    private readonly BonUnitOfWorkManager _bonUnitOfWorkManager;

    public AlwaysDisableTransactionsBonUnitOfWorkManager(BonUnitOfWorkManager bonUnitOfWorkManager)
    {
        _bonUnitOfWorkManager = bonUnitOfWorkManager;
    }

    public IBonUnitOfWork? Current => _bonUnitOfWorkManager.Current;

    public IBonUnitOfWork Begin(BonUnitOfWorkOptions options, bool requiresNew = false)
    {
        options.IsTransactional = false;
        return _bonUnitOfWorkManager.Begin(options, requiresNew);
    }

    public IBonUnitOfWork Reserve(string reservationName, bool requiresNew = false)
    {
        return _bonUnitOfWorkManager.Reserve(reservationName, requiresNew);
    }

    public void BeginReserved(string reservationName, BonUnitOfWorkOptions options)
    {
        options.IsTransactional = false;
        _bonUnitOfWorkManager.BeginReserved(reservationName, options);
    }

    public bool TryBeginReserved(string reservationName, BonUnitOfWorkOptions options)
    {
        options.IsTransactional = false;
        return _bonUnitOfWorkManager.TryBeginReserved(reservationName, options);
    }
}
