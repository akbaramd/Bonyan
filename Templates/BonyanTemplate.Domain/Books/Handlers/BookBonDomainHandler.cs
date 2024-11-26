using Bonyan.Messaging.Abstractions;
using BonyanTemplate.Domain.Books.DomainEvents;

namespace BonyanTemplate.Domain.Books.Handlers;

public class BookBonDomainHandler : IBonEventHandler<BookCreated>
{
 

    public Task HandleAsync(BookCreated @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("BookBonDomainHandler");
        throw new NotImplementedException();
    }
}