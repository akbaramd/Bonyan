namespace Bonyan.UnitOfWork;

public class NullUnitOfWorkTransactionBehaviourProvider : IUnitOfWorkTransactionBehaviourProvider
{
    public bool? IsTransactional => null;
}
