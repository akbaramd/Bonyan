using Bonyan.Messaging.Abstractions;
using BonyanTemplate.Domain.Books.DomainEvents;

namespace BonyanTemplate.Application.Consumers;

public class BookConsumer : IBonMessageConsumer<BookCreated>
{
    public Task ConsumeAsync(BonMessageContext<BookCreated> context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}