using Bonyan.Layer.Domain.Aggregate.Abstractions;

namespace Bonyan.Layer.Domain.Aggregate;

public abstract class BonModificationAggregateRoot : BonAggregateRoot, IBonModificationAggregateRoot
{
    public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
}

public abstract class BonModificationAggregateRoot<TId> : BonAggregateRoot<TId>,
    IBonModificationAggregateRoot<TId>
{
    public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
}