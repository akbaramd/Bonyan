using Bonyan.Layer.Domain.Abstractions;

namespace Bonyan.Layer.Application.Dto;

public abstract class ModificationAuditableAggregateRootDto : CreationAuditableAggregateRootDto, IModificationAuditable
{
  public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
}

public abstract class ModificationAuditableAggregateRootDto<TId> : CreationAuditableAggregateRootDto<TId>, IModificationAuditable
{
  public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
}
