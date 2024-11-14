using Bonyan.Layer.Domain.Audit.Abstractions;

namespace Bonyan.Layer.Domain.Aggregate.Abstractions;

public interface IBonSoftDeleteAggregateRoot : IBonAggregateRoot, IBonSoftDeleteAuditable
{
}

public interface IBonSoftDeleteAggregateRoot<TKey> : IBonAggregateRoot<TKey>, IBonSoftDeleteAuditable
{
}