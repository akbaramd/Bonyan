﻿using Bonyan.DomainDrivenDesign.Domain.Abstractions;

namespace Bonyan.DomainDrivenDesign.Domain.Aggregates;

public abstract class UpdateAuditableAggregateRoot : CreationAuditableAggregateRoot, IUpdateAuditable
{
  public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
}

public abstract class UpdateAuditableAggregateRoot<TId> : CreationAuditableAggregateRoot<TId>, IUpdateAuditable
{
  public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
}
