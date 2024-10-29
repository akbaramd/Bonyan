using Bonyan.Layer.Domain.Abstractions;

namespace Bonyan.Layer.Application.Dto;

public abstract class CreationAuditableAggregateRootDto : AggregateRootDto, ICreationAuditable
{
  public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

public abstract class CreationAuditableAggregateRootDto<TId> : AggregateRootDto<TId>, ICreationAuditable
{
  public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
