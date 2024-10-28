using Bonyan.Layer.Domain.Entities;
using Bonyan.Layer.Domain.Events;

namespace Bonyan.Layer.Domain.Aggregates;

public interface IAggregateRoot : IEntity
{
  public IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
  public void ClearEvents();
}

public interface IAggregateRoot<TKey> : IAggregateRoot, IEntity<TKey>
{
}
