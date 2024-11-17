using Bonyan.EntityFrameworkCore;

namespace Bonyan.UnitOfWork;
public class EfCoreBonDatabaseApi : IBonDatabaseApi, ISupportsSavingChanges
{
  public IEfDbContext DbContext { get; }

  public EfCoreBonDatabaseApi(IEfDbContext dbContext)
  {
    DbContext = dbContext;
  }

  public Task SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return DbContext.SaveChangesAsync(cancellationToken);
  }
}
