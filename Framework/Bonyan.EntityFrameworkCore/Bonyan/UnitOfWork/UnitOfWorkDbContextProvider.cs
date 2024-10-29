using Bonyan.EntityFrameworkCore;
using Bonyan.EntityFrameworkCore.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
                await CreateDbContextAsync(CancellationToken.None)
            );

            _unitOfWork.AddDatabaseApi(dbContextKey, databaseApi);
        }

        return (TDbContext)((EfCoreDatabaseApi)databaseApi).DbContext;
    }

    private async Task<IEfCoreDbContext> CreateDbContextAsync(CancellationToken cancellationToken)
    {
        var transactionApiKey = $"EntityFrameworkCore_{typeof(TDbContext)}";

        var activeTransaction = _unitOfWork.FindTransactionApi(transactionApiKey) as EfCoreTransactionApi;
        if (activeTransaction == null)
        {
            var dbContext = _serviceProvider.GetRequiredService<TDbContext>();
            var transaction = await dbContext.Database.BeginTransactionAsync();
            _unitOfWork.AddTransactionApi(transactionApiKey, new EfCoreTransactionApi(transaction, dbContext));
            return dbContext;
        }
        else
        {
            var dbContext = _serviceProvider.GetRequiredService<TDbContext>();
            if (dbContext.As<DbContext>().HasRelationalTransactionManager())
            {
                await dbContext.Database.BeginTransactionAsync(
                    cancellationToken
                );
            }
            else
            {
                try
                {
                    /* No need to store the returning IDbContextTransaction for non-relational databases
                     * since EfCoreTransactionApi will handle the commit/rollback over the DbContext instance.
                     */
                    await dbContext.Database.BeginTransactionAsync(cancellationToken);
                }
                catch (Exception e) when (e is InvalidOperationException || e is NotSupportedException)
                {
                    return dbContext;
                }
            }

            return dbContext;
        }
    }
}