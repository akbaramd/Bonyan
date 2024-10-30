namespace Bonyan.UnitOfWork;

public interface IUnitOfWorkTransactionBehaviourProvider
{
    bool? IsTransactional { get; }
}
