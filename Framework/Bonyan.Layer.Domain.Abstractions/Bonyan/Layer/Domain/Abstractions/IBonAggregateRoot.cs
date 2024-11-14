namespace Bonyan.Layer.Domain.Abstractions;

public interface IBonAggregateRoot : IBonEntity
{
    public IReadOnlyCollection<IBonDomainEvent> DomainEvents { get; }
    public void ClearEvents();
}

public interface IBonAggregateRoot<TKey> : IBonAggregateRoot, IBonEntity<TKey>
{
}