using Bonyan.Layer.Domain.Abstractions;

namespace Bonyan.Layer.Domain.Aggregates;

public abstract class BonModificationAuditableAggregateRoot : BonCreationAuditableAggregateRoot, IBonModificationAuditable
{
    public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
}

public abstract class BonModificationAuditableAggregateRoot<TId> : BonCreationAuditableAggregateRoot<TId>,
    IBonModificationAuditable
{
    public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
}