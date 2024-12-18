﻿using Bonyan.Layer.Domain.Audit.Abstractions;

namespace Bonyan.Layer.Application.Dto;

public abstract class BonFullAuditableAggregateRootDto : BonModificationAuditableAggregateRootDto, IBonFullAuditable
{
    public bool IsDeleted { get; set; }
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

public abstract class BonFullAuditableAggregateRootDto<TId> : BonModificationAuditableAggregateRootDto<TId>,
    IBonFullAuditable
{
    public bool IsDeleted { get; set; }
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