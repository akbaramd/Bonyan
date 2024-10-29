public interface ISupportsRollback
{
  Task RollbackAsync(CancellationToken cancellationToken = default);
}
