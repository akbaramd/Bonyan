using Bonyan.Layer.Domain.Abstractions;

namespace Bonyan.Layer.Domain.Aggregates;

public abstract class ModifiationAuditableAggregateRoot : CreationAuditableAggregateRoot, IModificationAuditable
{
    public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
}

public abstract class ModifiationAuditableAggregateRoot<TId> : CreationAuditableAggregateRoot<TId>,
    IModificationAuditable
{
    public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
}