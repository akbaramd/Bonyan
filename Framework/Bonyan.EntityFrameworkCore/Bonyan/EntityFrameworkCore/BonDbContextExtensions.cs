using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Bonyan.EntityFrameworkCore;

internal static class BonDbContextExtensions
{
  public static bool HasRelationalTransactionManager(this DbContext dbContext)
  {
    return dbContext.Database.GetService<IDbContextTransactionManager>() is IRelationalTransactionManager;
  }
}
