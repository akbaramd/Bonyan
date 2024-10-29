
namespace Bonyan.UnitOfWork
{
  public interface IUnitOfWork : ITransactionApiContainer,IDatabaseApiContainer,IDisposable
  {
   
    // Commit the transaction asynchronously
    Task CommitAsync(CancellationToken? cancellationToken = default);

    // Rollback the transaction
    Task RollbackAsync(CancellationToken cancellationToken = default);

    // Check if a transaction is currently active

    // Save changes asynchronously
    Task SaveChangesAsync(CancellationToken? cancellationToken = default);


  }
}
