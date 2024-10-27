using Bonyan.DomainDrivenDesign.Domain.Events;
using BonyanTemplate.Domain.DomainEvents;

namespace BonyanTemplate.Domain.Handlers;

public class BookDomainHandler : IDomainEventHandler<BookCreated>
{
  public Task Handle(BookCreated domainEvent)
  {
    throw new NotImplementedException();
  }
}
