using Bonyan.Layer.Domain.Aggregate.Abstractions;

namespace Bonyan.Layer.Domain.Aggregate;

public abstract class BonCreationAggregateRoot : BonAggregateRoot, IBonCreationAggregateRoot
{
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

public abstract class BonCreationAggregateRoot<TId> : BonAggregateRoot<TId>, IBonCreationAggregateRoot<TId>
{
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}