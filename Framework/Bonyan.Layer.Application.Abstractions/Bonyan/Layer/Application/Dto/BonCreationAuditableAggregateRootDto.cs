using Bonyan.Layer.Domain.Audit.Abstractions;

namespace Bonyan.Layer.Application.Dto;

public abstract class BonCreationAuditableAggregateRootDto : BonAggregateRootDto, IBonCreationAuditable
{
  public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

public abstract class BonCreationAuditableAggregateRootDto<TId> : BonAggregateRootDto<TId>, IBonCreationAuditable
{
  public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
