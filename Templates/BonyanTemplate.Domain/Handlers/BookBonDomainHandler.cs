using Bonyan.Layer.Domain.Abstractions;
using Bonyan.Layer.Domain.Events;
using BonyanTemplate.Domain.DomainEvents;

namespace BonyanTemplate.Domain.Handlers;

public class BookBonDomainHandler : IBonDomainEventHandler<BookCreated>
{
    public Task HandleAsync(BookCreated domainEvent , CancellationToken? cancellationToken )
    {
        throw new NotImplementedException();
    }
}