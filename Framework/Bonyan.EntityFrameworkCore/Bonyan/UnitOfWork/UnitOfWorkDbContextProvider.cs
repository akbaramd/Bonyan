using Bonyan.EntityFrameworkCore;
using Bonyan.EntityFrameworkCore.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.UnitOfWork;

public class UnitOfWorkDbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
    where TDbContext : DbContext, IBonyanDbContext<TDbContext>
{
    private const string TransactionsNotSupportedWarningMessage = "Current database does not support transactions. Your database may remain in an inconsistent state in an error case.";
    
    private readonly IServiceProvider _serviceProvider;
    protected readonly IUnitOfWorkManager UnitOfWorkManager;

    public UnitOfWorkDbContextProvider(IServiceProvider serviceProvider, IUnitOfWorkManager unitOfWork)
    {
        _serviceProvider = serviceProvider;
        UnitOfWorkManager = unitOfWork;
    }

    public async Task<TDbContext> GetDbContextAsync()
    {
        var unitOfWork = UnitOfWorkManager.Current;
        if (unitOfWork == null)
        {
            throw new Exception("A DbContext can only be created inside a unit of work!");
        }

        var dbContextKey = $"{typeof(TDbContext).FullName}";

        var databaseApi = unitOfWork.FindDatabaseApi(dbContextKey);

        if (databaseApi == null)
        {
            databaseApi = new EfCoreDatabaseApi(
                await CreateDbContextAsync(unitOfWork, CancellationToken.None)
            );

            unitOfWork.AddDatabaseApi(dbContextKey, databaseApi);
        }

        return (TDbContext)((EfCoreDatabaseApi)databaseApi).DbContext;
    }

    private async Task<IEfCoreDbContext> CreateDbContextAsync(IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var transactionApiKey = $"EntityFrameworkCore_{typeof(TDbContext)}";
        var activeTransaction = unitOfWork.FindTransactionApi(transactionApiKey) as EfCoreTransactionApi;
        if (activeTransaction == null)
        {
            var dbContext = _serviceProvider.GetRequiredService<TDbContext>();

            try
            {
                var dbTransaction = unitOfWork.Options.IsolationLevel.HasValue
                    ? await dbContext.Database.BeginTransactionAsync(unitOfWork.Options.IsolationLevel.Value,
                        cancellationToken)
                    : await dbContext.Database.BeginTransactionAsync(cancellationToken);
                unitOfWork.AddTransactionApi(transactionApiKey, new EfCoreTransactionApi(dbTransaction, dbContext));
            }
            catch (Exception e)
            {
                Console.WriteLine(TransactionsNotSupportedWarningMessage);
                return dbContext;
            }
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