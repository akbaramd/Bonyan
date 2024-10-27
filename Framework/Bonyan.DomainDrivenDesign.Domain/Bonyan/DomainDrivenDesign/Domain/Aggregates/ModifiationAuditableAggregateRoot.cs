using Bonyan.DomainDrivenDesign.Domain.Abstractions;

namespace Bonyan.DomainDrivenDesign.Domain.Aggregates;

public abstract class ModifiationAuditableAggregateRoot : CreationAuditableAggregateRoot, IModificationAuditable
{
  public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
}

public abstract class ModifiationAuditableAggregateRoot<TId> : CreationAuditableAggregateRoot<TId>, IModificationAuditable
{
  public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
}
