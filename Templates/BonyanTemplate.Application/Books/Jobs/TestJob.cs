using Bonyan.Workers;
using BonyanTemplate.Domain.Books;

namespace BonyanTemplate.Application.Books.Jobs;

[CronJob("*/1 * * * *")]
public class BookOutOfStockNotifierWorker(IBooksRepository userRepository) : IBonWorker
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var res = await userRepository.FindAsync(x=>x.Status == BookStatus.OutOfStock);

        foreach (var book in res)
        {
            Console.WriteLine($"Book {book.Title} is outOfStock");
        }
    }
}