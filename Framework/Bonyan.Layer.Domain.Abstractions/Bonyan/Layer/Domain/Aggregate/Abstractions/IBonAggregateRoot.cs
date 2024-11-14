using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Layer.Domain.Entity;

namespace Bonyan.Layer.Domain.Aggregate.Abstractions;

public interface IBonAggregateRoot : IBonEntity
{
    public IReadOnlyCollection<IBonDomainEvent> DomainEvents { get; }
    public void ClearEvents();
}

public interface IBonAggregateRoot<TKey> : IBonAggregateRoot, IBonEntity<TKey>
{
}