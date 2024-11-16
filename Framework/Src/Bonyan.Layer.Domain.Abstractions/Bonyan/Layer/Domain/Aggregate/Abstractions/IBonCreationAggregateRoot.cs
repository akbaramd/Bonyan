using Bonyan.Layer.Domain.Audit.Abstractions;

namespace Bonyan.Layer.Domain.Aggregate.Abstractions;

public interface IBonCreationAggregateRoot<TKey> : IBonAggregateRoot<TKey>, IBonCreationAuditable
{
}

public interface IBonCreationAggregateRoot : IBonAggregateRoot, IBonCreationAuditable
{
}

public interface IBonModificationAggregateRoot : IBonAggregateRoot, IBonModificationAuditable
{
}