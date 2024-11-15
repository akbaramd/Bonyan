using Bonyan.Messaging.Abstractions;
using BonyanTemplate.Domain.DomainEvents;

namespace BonyanTemplate.Domain.Handlers;

public class BookBonDomainHandler : IBonMessageConsumer<BookCreated>
{
    public Task ConsumeAsync(BookCreated domainEvent , CancellationToken? cancellationToken )
    {
        throw new NotImplementedException();
    }
}