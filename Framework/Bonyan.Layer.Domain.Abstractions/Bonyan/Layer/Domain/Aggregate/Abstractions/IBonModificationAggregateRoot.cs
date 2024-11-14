using Bonyan.Layer.Domain.Audit.Abstractions;

namespace Bonyan.Layer.Domain.Aggregate.Abstractions;

public interface IBonModificationAggregateRoot<TKey> : IBonAggregateRoot<TKey>, IBonModificationAuditable
{
}