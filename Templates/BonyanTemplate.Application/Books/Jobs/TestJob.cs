using Bonyan.Workers;
using BonyanTemplate.Domain.Books;

namespace BonyanTemplate.Application.Books.Jobs;

/// <summary>
/// Background worker that notifies when books are out of stock. Runs on a cron schedule.
/// </summary>
[CronJob("*/1 * * * *")]
public class BookOutOfStockNotifierWorker(IBooksRepository booksRepository) : IBonWorker
{
    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Book> outOfStock = await booksRepository.FindAsync(x => x.Status == BookStatus.OutOfStock);
        foreach (Book book in outOfStock)
        {
            Console.WriteLine($"Book {book.Title} is out of stock.");
        }
    }
}