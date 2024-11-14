using Bonyan.Layer.Domain.Audit.Abstractions;

namespace Bonyan.Layer.Domain.Aggregate.Abstractions;

public interface IBonFullAggregateRoot<TKey> : IBonFullAggregateRoot, IBonCreationAggregateRoot<TKey>,
    IBonModificationAggregateRoot<TKey>, IBonSoftDeleteAggregateRoot<TKey>
{
}

public interface IBonFullAggregateRoot : IBonFullAuditable, IBonCreationAggregateRoot, IBonModificationAggregateRoot,
    IBonSoftDeleteAggregateRoot
{
}