using Bonyan.EntityFrameworkCore;

namespace Bonyan.UnitOfWork;
public class EfCoreBonDatabaseApi : IBonDatabaseApi, ISupportsSavingChanges
{
  public IBonEfCoreDbContext DbContext { get; }

  public EfCoreBonDatabaseApi(IBonEfCoreDbContext dbContext)
  {
    DbContext = dbContext;
  }

  public Task SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return DbContext.SaveChangesAsync(cancellationToken);
  }
}
