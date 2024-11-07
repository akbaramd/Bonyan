namespace Bonyan.UnitOfWork;

public interface IBonTransactionApi : IDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
