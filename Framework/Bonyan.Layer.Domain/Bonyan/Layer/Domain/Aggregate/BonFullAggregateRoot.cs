using Bonyan.Layer.Domain.Aggregate.Abstractions;
using Bonyan.Layer.Domain.Audit.Abstractions;
using System;

namespace Bonyan.Layer.Domain.Aggregate;

/// <summary>
/// Base class for aggregate roots with full audit and soft deletion support.
/// </summary>
public abstract class BonFullAggregateRoot : BonAggregateRoot, IBonFullAggregateRoot
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; } // Made private set for encapsulation

    public DateTime CreatedDate { get; set; } // Private set to enforce control
    public DateTime? ModifiedDate { get; set; } // Nullable for optional modification tracking

    protected BonFullAggregateRoot()
    {
        CreatedDate = DateTime.UtcNow;
        IsDeleted = false;
    }

    /// <summary>
    /// Marks the entity as deleted and sets the DeletedDate.
    /// </summary>
    public void SoftDelete()
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            DeletedDate = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Restores a soft-deleted entity by unsetting IsDeleted and DeletedDate.
    /// </summary>
    public void Restore()
    {
        if (IsDeleted)
        {
            IsDeleted = false;
            DeletedDate = null;
        }
    }

    /// <summary>
    /// Updates the ModifiedDate, typically called whenever the entity is modified.
    /// </summary>
    public void UpdateModifiedDate()
    {
        ModifiedDate = DateTime.UtcNow;
    }
}

/// <summary>
/// Generic base class for aggregate roots with full audit and soft deletion support.
/// </summary>
/// <typeparam name="TId">The type of the aggregate root's identifier.</typeparam>
public abstract class BonFullAggregateRoot<TId> : BonAggregateRoot<TId>, IBonFullAggregateRoot<TId>
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; } // Made private set for encapsulation

    public DateTime CreatedDate { get; set; } // Private set to enforce control
    public DateTime? ModifiedDate { get; set; } // Nullable for optional modification tracking

    protected BonFullAggregateRoot () {}
    protected BonFullAggregateRoot(TId id)

    {
        CreatedDate = DateTime.UtcNow;
        IsDeleted = false;
    }

    /// <summary>
    /// Marks the entity as deleted and sets the DeletedDate.
    /// </summary>
    public void SoftDelete()
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            DeletedDate = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Restores a soft-deleted entity by unsetting IsDeleted and DeletedDate.
    /// </summary>
    public void Restore()
    {
        if (IsDeleted)
        {
            IsDeleted = false;
            DeletedDate = null;
        }
    }

    /// <summary>
    /// Updates the ModifiedDate, typically called whenever the entity is modified.
    /// </summary>
    public void UpdateModifiedDate()
    {
        ModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the CreatedDate only if it has not been set previously.
    /// </summary>
    protected void SetCreationDate(DateTime creationDate)
    {
        if (CreatedDate == default)
        {
            CreatedDate = creationDate;
        }
    }
}