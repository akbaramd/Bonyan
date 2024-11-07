using Bonyan.Layer.Domain.Entities;
using Bonyan.Layer.Domain.Events;

namespace Bonyan.Layer.Domain.Aggregates;

public interface IBonAggregateRoot : IBonEntity
{
    public IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    public void ClearEvents();
}

public interface IBonAggregateRoot<TKey> : IBonAggregateRoot, IBonEntity<TKey>
{
}