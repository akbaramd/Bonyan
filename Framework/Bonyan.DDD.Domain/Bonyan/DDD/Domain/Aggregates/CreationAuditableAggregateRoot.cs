using Bonyan.DDD.Domain.Abstractions;

namespace Bonyan.DDD.Domain.Aggregates;

public abstract class CreationAuditableAggregateRoot : AggregateRoot, ICreationAuditable
{
  public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

public abstract class CreationAuditableAggregateRoot<TId> : AggregateRoot<TId>, ICreationAuditable
{
  public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
