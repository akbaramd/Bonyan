using Bonyan.Layer.Domain.Events;
using BonyanTemplate.Domain.DomainEvents;

namespace BonyanTemplate.Domain.Handlers;

public class BookBonDomainHandler : IBonDomainEventHandler<BookCreated>
{
    public Task Handle(BookCreated domainEvent)
    {
        throw new NotImplementedException();
    }
}