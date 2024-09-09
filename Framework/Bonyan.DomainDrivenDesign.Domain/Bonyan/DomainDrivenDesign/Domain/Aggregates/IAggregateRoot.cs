using Bonyan.DomainDrivenDesign.Domain.Entities;

namespace Bonyan.DomainDrivenDesign.Domain.Aggregates;

public interface IAggregateRoot : IEntity
{
}

public interface IAggregateRoot<TKey> : IAggregateRoot, IEntity<TKey>
{
}
