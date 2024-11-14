using Bonyan.Layer.Domain.Aggregate.Abstractions;

namespace Bonyan.Layer.Domain.Aggregate;

public abstract class BonSoftDeleteAggregateRoot<TId> : BonAggregateRoot<TId>,
    IBonSoftDeleteAggregateRoot<TId>
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
}

public abstract class BonSoftDeleteAggregateRoot : BonAggregateRoot, IBonSoftDeleteAggregateRoot
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
}