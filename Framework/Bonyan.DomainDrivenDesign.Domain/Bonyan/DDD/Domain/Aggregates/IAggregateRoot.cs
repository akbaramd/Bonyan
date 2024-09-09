using Bonyan.DDD.Domain.Entities;

namespace Bonyan.DDD.Domain.Aggregates;

public interface IAggregateRoot : IEntity
{
}

public interface IAggregateRoot<TKey> : IAggregateRoot, IEntity<TKey>
{
}
