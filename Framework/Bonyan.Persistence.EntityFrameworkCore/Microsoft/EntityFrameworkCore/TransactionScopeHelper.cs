namespace Microsoft.EntityFrameworkCore;

public static class TransactionScopeHelper
{
  public static async Task ExecuteInTransactionAsync(DbContext dbContext, Func<Task> action)
  {
    await using var transaction = await dbContext.Database.BeginTransactionAsync();
    try
    {
      await action();
      await transaction.CommitAsync();
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }
}