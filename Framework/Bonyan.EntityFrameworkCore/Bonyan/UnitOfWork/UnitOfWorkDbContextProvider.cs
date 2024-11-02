using Bonyan.EntityFrameworkCore;
using Bonyan.EntityFrameworkCore.Abstractions;
using Bonyan.Exceptions;
using Bonyan.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bonyan.UnitOfWork;

public class UnitOfWorkDbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
    where TDbContext : DbContext, IBonyanDbContext<TDbContext>
{
    private const string TransactionsNotSupportedWarningMessage = "Current database does not support transactions. Your database may remain in an inconsistent state in an error case.";

    public ILogger<UnitOfWorkDbContextProvider<TDbContext>> Logger { get; set; }

    protected readonly IUnitOfWorkManager UnitOfWorkManager;
    protected readonly ICurrentTenant CurrentTenant;

    public UnitOfWorkDbContextProvider(
        IUnitOfWorkManager unitOfWorkManager,
        ICurrentTenant currentTenant)
    {
        UnitOfWorkManager = unitOfWorkManager;
        CurrentTenant = currentTenant;

        Logger = NullLogger<UnitOfWorkDbContextProvider<TDbContext>>.Instance;
    }


    public virtual async Task<TDbContext> GetDbContextAsync()
    {
        var unitOfWork = UnitOfWorkManager.Current;
        if (unitOfWork == null)
        {
            throw new BonyanException("A DbContext can only be created inside a unit of work!");
        }

        var targetDbContextType = (typeof(TDbContext));

        var dbContextKey = $"{targetDbContextType.FullName}";

        var databaseApi = unitOfWork.FindDatabaseApi(dbContextKey);

        if (databaseApi == null)
        {
            databaseApi = new EfCoreDatabaseApi(
                await CreateDbContextAsync(unitOfWork)
            );

            unitOfWork.AddDatabaseApi(dbContextKey, databaseApi);
        }

        return (TDbContext)((EfCoreDatabaseApi)databaseApi).DbContext;
    }

    protected virtual async Task<TDbContext> CreateDbContextAsync(IUnitOfWork unitOfWork)
    {
            var dbContext = await CreateDbContextAsync(unitOfWork,CancellationToken.None);

            return dbContext;
    }



    protected virtual async Task<TDbContext> CreateDbContextAsync(IUnitOfWork unitOfWork,CancellationToken cancellationToken)
    {
        return unitOfWork.Options.IsTransactional
            ?await CreateDbContextWithTransactionAsync(unitOfWork,cancellationToken)
            : unitOfWork.ServiceProvider.GetRequiredService<TDbContext>();
    }

 

    
    protected virtual async Task<TDbContext> CreateDbContextWithTransactionAsync(IUnitOfWork unitOfWork,CancellationToken cancellationToken)
    {
            var transactionApiKey = $"EntityFrameworkCore_{typeof(TDbContext).Name}";
        var activeTransaction = unitOfWork.FindTransactionApi(transactionApiKey) as EfCoreTransactionApi;

        if (activeTransaction == null)
        {
            var dbContext = unitOfWork.ServiceProvider.GetRequiredService<TDbContext>();

            try
            {
                var dbTransaction = unitOfWork.Options.IsolationLevel.HasValue
                    ? await dbContext.Database.BeginTransactionAsync(unitOfWork.Options.IsolationLevel.Value, cancellationToken)
                    : await dbContext.Database.BeginTransactionAsync(cancellationToken);

                unitOfWork.AddTransactionApi(
                    transactionApiKey,
                    new EfCoreTransactionApi(
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
           

            var dbContext = unitOfWork.ServiceProvider.GetRequiredService<TDbContext>();

            if (dbContext.As<DbContext>().HasRelationalTransactionManager())
            {
                
                    try
                    {
                        /* User did not re-use the ExistingConnection and we are starting a new transaction.
                            * EfCoreTransactionApi will check the connection string match and separately
                            * commit/rollback this transaction over the DbContext instance. */
                        if (unitOfWork.Options.IsolationLevel.HasValue)
                        {
                            await dbContext.Database.BeginTransactionAsync(
                                unitOfWork.Options.IsolationLevel.Value,
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
