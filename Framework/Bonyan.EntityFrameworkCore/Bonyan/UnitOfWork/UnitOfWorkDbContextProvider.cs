using Bonyan.EntityFrameworkCore;
using Bonyan.EntityFrameworkCore.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Uow.EntityFrameworkCore;

namespace Bonyan.UnitOfWork;

public class UnitOfWorkDbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
  where TDbContext : DbContext, IBonyanDbContext<TDbContext>
{
  private readonly IServiceProvider _serviceProvider;
  private readonly IUnitOfWork _unitOfWork;

  public UnitOfWorkDbContextProvider(IServiceProvider serviceProvider, IUnitOfWork unitOfWork)
  {
    _serviceProvider = serviceProvider;
    _unitOfWork = unitOfWork;
  }

  public async Task<TDbContext> GetDbContextAsync()
  {
    var dbContextKey = $"{typeof(TDbContext).FullName}";

    var databaseApi = _unitOfWork.FindDatabaseApi(dbContextKey);

    if (databaseApi == null)
    {
      databaseApi = new EfCoreDatabaseApi(
        await CreateDbContextAsync()
      );

      _unitOfWork.AddDatabaseApi(dbContextKey, databaseApi);
    }

    return (TDbContext)((EfCoreDatabaseApi)databaseApi).DbContext;
  }

  private async Task<IEfCoreDbContext> CreateDbContextAsync()
  {
    var transactionApiKey = $"EntityFrameworkCore_{typeof(TDbContext)}";
    var dbContext = _serviceProvider.GetRequiredService<TDbContext>();
    var transaction = await dbContext.Database.BeginTransactionAsync();
    _unitOfWork.AddTransactionApi(transactionApiKey, new EfCoreTransactionApi(transaction, dbContext));
    return dbContext;
  }
}
