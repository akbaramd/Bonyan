using Bonyan.Layer.Domain.Abstractions;

namespace Bonyan.Layer.Domain.Aggregates;

public abstract class BonFullAuditableAggregateRoot : BonModificationAuditableAggregateRoot, IBonFullAuditable
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

public abstract class BonFullAuditableAggregateRoot<TId> : BonModificationAuditableAggregateRoot<TId>, IBonFullAuditable
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