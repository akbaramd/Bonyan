namespace Bonyan.UnitOfWork;

public interface ISupportsRollback
{
  Task RollbackAsync(CancellationToken cancellationToken = default);
}