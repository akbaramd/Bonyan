using Bonyan.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Bonyan.UnitOfWork;

public class EfCoreBonTransactionApi : IBonTransactionApi, ISupportsRollback
{
    public IDbContextTransaction DbContextTransaction { get; }
    public IEfDbContext StarterDbContext { get; }
    public List<IEfDbContext> AttendedDbContexts { get; }


    public EfCoreBonTransactionApi(
        IDbContextTransaction dbContextTransaction,
        IEfDbContext starterDbContext)
    {
        DbContextTransaction = dbContextTransaction;
        StarterDbContext = starterDbContext;
        AttendedDbContexts = new List<IEfDbContext>();
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
