using Bonyan.Layer.Domain.Audit.Abstractions;

namespace Bonyan.Layer.Application.Dto;

public abstract class BonFullAuditableAggregateRootDto : BonModificationAuditableAggregateRootDto, IBonFullAuditable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public void SoftDelete()
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }
    }

    public void Restore()
    {
        if (IsDeleted)
        {
            IsDeleted = false;
            DeletedAt = null;
        }
    }
}

public abstract class BonFullAuditableAggregateRootDto<TId> : BonModificationAuditableAggregateRootDto<TId>,
    IBonFullAuditable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public void SoftDelete()
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }
    }

    public void Restore()
    {
        if (IsDeleted)
        {
            IsDeleted = false;
            DeletedAt = null;
        }
    }
}