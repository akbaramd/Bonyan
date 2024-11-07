using Bonyan.EntityFrameworkCore;
using Bonyan.EntityFrameworkCore.Abstractions;
using Bonyan.Exceptions;
using Bonyan.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bonyan.UnitOfWork;

public class BonUnitOfWorkBonDbContextProvider<TDbContext> : IBonDbContextProvider<TDbContext>
    where TDbContext : DbContext, IBonDbContext<TDbContext>
{
    private const string TransactionsNotSupportedWarningMessage = "Current database does not support transactions. Your database may remain in an inconsistent state in an error case.";

    public ILogger<BonUnitOfWorkBonDbContextProvider<TDbContext>> Logger { get; set; }

    protected readonly IBonUnitOfWorkManager BonUnitOfWorkManager;
    protected readonly IBonCurrentTenant BonCurrentTenant;

    public BonUnitOfWorkBonDbContextProvider(
        IBonUnitOfWorkManager bonUnitOfWorkManager,
        IBonCurrentTenant bonCurrentTenant)
    {
        BonUnitOfWorkManager = bonUnitOfWorkManager;
        BonCurrentTenant = bonCurrentTenant;

        Logger = NullLogger<BonUnitOfWorkBonDbContextProvider<TDbContext>>.Instance;
    }


    public virtual async Task<TDbContext> GetDbContextAsync()
    {
        var unitOfWork = BonUnitOfWorkManager.Current;
        if (unitOfWork == null)
        {
            throw new BonException("A DbContext can only be created inside a unit of work!");
        }

        var targetDbContextType = (typeof(TDbContext));

        var dbContextKey = $"{targetDbContextType.FullName}";

        var databaseApi = unitOfWork.FindDatabaseApi(dbContextKey);

        if (databaseApi == null)
        {
            databaseApi = new EfCoreBonDatabaseApi(
                await CreateDbContextAsync(unitOfWork)
            );

            unitOfWork.AddDatabaseApi(dbContextKey, databaseApi);
        }

        return (TDbContext)((EfCoreBonDatabaseApi)databaseApi).DbContext;
    }

    protected virtual async Task<TDbContext> CreateDbContextAsync(IBonUnitOfWork bonUnitOfWork)
    {
            var dbContext = await CreateDbContextAsync(bonUnitOfWork,CancellationToken.None);

            return dbContext;
    }



    protected virtual async Task<TDbContext> CreateDbContextAsync(IBonUnitOfWork bonUnitOfWork,CancellationToken cancellationToken)
    {
        return bonUnitOfWork.Options.IsTransactional
            ?await CreateDbContextWithTransactionAsync(bonUnitOfWork,cancellationToken)
            : bonUnitOfWork.ServiceProvider.GetRequiredService<TDbContext>();
    }

 

    
    protected virtual async Task<TDbContext> CreateDbContextWithTransactionAsync(IBonUnitOfWork bonUnitOfWork,CancellationToken cancellationToken)
    {
            var transactionApiKey = $"EntityFrameworkCore_{typeof(TDbContext).Name}";
        var activeTransaction = bonUnitOfWork.FindTransactionApi(transactionApiKey) as EfCoreBonTransactionApi;

        if (activeTransaction == null)
        {
            var dbContext = bonUnitOfWork.ServiceProvider.GetRequiredService<TDbContext>();

            try
            {
                var dbTransaction = bonUnitOfWork.Options.IsolationLevel.HasValue
                    ? await dbContext.Database.BeginTransactionAsync(bonUnitOfWork.Options.IsolationLevel.Value, cancellationToken)
                    : await dbContext.Database.BeginTransactionAsync(cancellationToken);

                bonUnitOfWork.AddTransactionApi(
                    transactionApiKey,
                    new EfCoreBonTransactionApi(
                        dbTransaction,
                        dbContext
                    )
                );
            }
            catch (Exception e) when (e is InvalidOperationException || e is NotSupportedException)
            {
                Logger.LogWarning(TransactionsNotSupportedWarningMessage);

                return dbContext;
            }

            return dbContext;
        }
        else
        {
           

            var dbContext = bonUnitOfWork.ServiceProvider.GetRequiredService<TDbContext>();

            if (dbContext.As<DbContext>().HasRelationalTransactionManager())
            {
                
                    try
                    {
                        /* User did not re-use the ExistingConnection and we are starting a new transaction.
                            * EfCoreTransactionApi will check the connection string match and separately
                            * commit/rollback this transaction over the DbContext instance. */
                        if (bonUnitOfWork.Options.IsolationLevel.HasValue)
                        {
                            await dbContext.Database.BeginTransactionAsync(
                                bonUnitOfWork.Options.IsolationLevel.Value,
                                cancellationToken
                            );
                        }
                        else
                        {
                            await dbContext.Database.BeginTransactionAsync(
                                cancellationToken
                            );
                        }
                    }
                    catch (Exception e) when (e is InvalidOperationException || e is NotSupportedException)
                    {
                        Logger.LogWarning(TransactionsNotSupportedWarningMessage);

                        return dbContext;
                    }
              
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
                    Logger.LogWarning(TransactionsNotSupportedWarningMessage);

                    return dbContext;
                }
            }

            activeTransaction.AttendedDbContexts.Add(dbContext);

            return dbContext;
        }
    }
   
  
}
