using Bonyan.Layer.Domain.Abstractions;

namespace Bonyan.Layer.Domain.Aggregates;

public abstract class BonCreationAuditableAggregateRoot : BonAggregateRoot, IBonCreationAuditable
{
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

public abstract class BonCreationAuditableAggregateRoot<TId> : BonAggregateRoot<TId>, IBonCreationAuditable
{
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}