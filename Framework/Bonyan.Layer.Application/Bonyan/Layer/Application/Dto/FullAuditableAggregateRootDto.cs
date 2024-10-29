﻿using Bonyan.Layer.Domain.Abstractions;

namespace Bonyan.Layer.Application.Dto;

public abstract class FullAuditableAggregateRootDto : ModificationAuditableAggregateRootDto, IFullAuditable
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

public abstract class FullAuditableAggregateRootDto<TId> : ModificationAuditableAggregateRootDto<TId>, IFullAuditable
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