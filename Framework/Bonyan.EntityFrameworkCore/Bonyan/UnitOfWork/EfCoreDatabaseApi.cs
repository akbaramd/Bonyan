using Bonyan.EntityFrameworkCore;
using Volo.Abp.Uow;

namespace Bonyan.UnitOfWork;
public class EfCoreDatabaseApi : IDatabaseApi, ISupportsSavingChanges
{
  public IEfCoreDbContext DbContext { get; }

  public EfCoreDatabaseApi(IEfCoreDbContext dbContext)
  {
    DbContext = dbContext;
  }

  public Task SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return DbContext.SaveChangesAsync(cancellationToken);
  }
}
