using Bonyan.DDD.Domain.Abstractions;

namespace Bonyan.DDD.Domain.Aggregates;

public abstract class FullAuditableAggregateRoot : UpdateAuditableAggregateRoot, IFullAuditable
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

public abstract class FullAuditableAggregateRoot<TId> : UpdateAuditableAggregateRoot<TId>, IFullAuditable
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
