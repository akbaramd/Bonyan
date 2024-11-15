﻿using Bonyan.Layer.Domain.Audit.Abstractions;

namespace Bonyan.Layer.Application.Dto;

public abstract class BonModificationAuditableAggregateRootDto : BonCreationAuditableAggregateRootDto, IBonModificationAuditable
{
  public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
}

public abstract class BonModificationAuditableAggregateRootDto<TId> : BonCreationAuditableAggregateRootDto<TId>, IBonModificationAuditable
{
  public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
}
