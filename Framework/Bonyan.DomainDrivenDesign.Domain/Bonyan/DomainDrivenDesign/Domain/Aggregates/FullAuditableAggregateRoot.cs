﻿using Bonyan.DomainDrivenDesign.Domain.Abstractions;

namespace Bonyan.DomainDrivenDesign.Domain.Aggregates;

public abstract class FullAuditableAggregateRoot : ModifiationAuditableAggregateRoot, IFullAuditable
{
  public bool IsDeleted { get; private set; }
  public DateTime? DeletedDate { get; set; }

  public void SoftDelete()
  {
    if (!IsDeleted)
    {
      IsDeleted = true;
      DeletedDate = DateTime.UtcNow;
    }
  }

  public void Restore()
  {
    if (IsDeleted)
    {
      IsDeleted = false;
      DeletedDate = null;
    }
  }
}

public abstract class FullAuditableAggregateRoot<TId> : ModifiationAuditableAggregateRoot<TId>, IFullAuditable
{
  public bool IsDeleted { get; private set; }
  public DateTime? DeletedDate { get; set; }

  public void SoftDelete()
  {
    if (!IsDeleted)
    {
      IsDeleted = true;
      DeletedDate = DateTime.UtcNow;
    }
  }

  public void Restore()
  {
    if (IsDeleted)
    {
      IsDeleted = false;
      DeletedDate = null;
    }
  }
}
