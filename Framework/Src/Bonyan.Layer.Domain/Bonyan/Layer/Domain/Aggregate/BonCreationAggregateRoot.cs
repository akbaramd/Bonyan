using Bonyan.Layer.Domain.Aggregate.Abstractions;

namespace Bonyan.Layer.Domain.Aggregate;

public abstract class BonCreationAggregateRoot : BonAggregateRoot, IBonCreationAggregateRoot
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public abstract class BonCreationAggregateRoot<TId> : BonAggregateRoot<TId>, IBonCreationAggregateRoot<TId>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}