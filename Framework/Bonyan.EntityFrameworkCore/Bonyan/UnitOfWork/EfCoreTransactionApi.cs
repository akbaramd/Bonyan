using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Volo.Abp.Uow.EntityFrameworkCore;

public class EfCoreTransactionApi : ITransactionApi, ISupportsRollback
{
    public IDbContextTransaction DbContextTransaction { get; }
    public IEfCoreDbContext StarterDbContext { get; }
    public List<IEfCoreDbContext> AttendedDbContexts { get; }


    public EfCoreTransactionApi(
        IDbContextTransaction dbContextTransaction,
        IEfCoreDbContext starterDbContext)
    {
        DbContextTransaction = dbContextTransaction;
        StarterDbContext = starterDbContext;
        AttendedDbContexts = new List<IEfCoreDbContext>();
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        foreach (var dbContext in AttendedDbContexts)
        {
            if (dbContext.As<DbContext>().HasRelationalTransactionManager() &&
                dbContext.Database.GetDbConnection() == DbContextTransaction.GetDbTransaction().Connection)
            {
                continue; //Relational databases use the shared transaction if they are using the same connection
            }

            await dbContext.Database.CommitTransactionAsync(cancellationToken);
        }

        await DbContextTransaction.CommitAsync(cancellationToken);
    }

    public void Dispose()
    {
        DbContextTransaction.Dispose();
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        foreach (var dbContext in AttendedDbContexts)
        {
            if (dbContext.As<DbContext>().HasRelationalTransactionManager() &&
                dbContext.Database.GetDbConnection() == DbContextTransaction.GetDbTransaction().Connection)
            {
                continue; //Relational databases use the shared transaction if they are using the same connection
            }

            await dbContext.Database.RollbackTransactionAsync(cancellationToken);
        }

        await DbContextTransaction.RollbackAsync(cancellationToken);
    }
}
