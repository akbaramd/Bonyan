using Bonyan.DomainDrivenDesign.Domain.Entities;
using Bonyan.DomainDrivenDesign.Domain.Events;

namespace Bonyan.DomainDrivenDesign.Domain.Aggregates;

public interface IAggregateRoot : IEntity
{
  public IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
  public void ClearEvents();
}

public interface IAggregateRoot<TKey> : IAggregateRoot, IEntity<TKey>
{
}
