namespace Bonyan.UnitOfWork;

public interface ISupportsSavingChanges
{
  Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
