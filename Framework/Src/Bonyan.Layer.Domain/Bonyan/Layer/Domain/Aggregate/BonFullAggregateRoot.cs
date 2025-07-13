using Bonyan.Layer.Domain.Aggregate.Abstractions;

namespace Bonyan.Layer.Domain.Aggregate;

/// <summary>
/// Base class for aggregate roots with full audit and soft deletion support.
/// </summary>
public abstract class BonFullAggregateRoot : BonAggregateRoot, IBonFullAggregateRoot
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; } // Made private set for encapsulation

    public DateTime CreatedAt { get; set; } // Private set to enforce control
    public DateTime? ModifiedAt { get; set; } // Nullable for optional modification tracking

    protected BonFullAggregateRoot()
    {
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    /// <summary>
    /// Marks the entity as deleted and sets the DeletedAt.
    /// </summary>
    public void SoftDelete()
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Restores a soft-deleted entity by unsetting IsDeleted and DeletedAt.
    /// </summary>
    public void Restore()
    {
        if (IsDeleted)
        {
            IsDeleted = false;
            DeletedAt = null;
        }
    }

    /// <summary>
    /// Updates the ModifiedAt, typically called whenever the entity is modified.
    /// </summary>
    public void UpdateModifiedDate()
    {
        ModifiedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Generic base class for aggregate roots with full audit and soft deletion support.
/// </summary>
/// <typeparam name="TId">The type of the aggregate root's identifier.</typeparam>
public abstract class BonFullAggregateRoot<TId> : BonAggregateRoot<TId>, IBonFullAggregateRoot<TId>
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; } // Made private set for encapsulation

    public DateTime CreatedAt { get; set; } // Private set to enforce control
    public DateTime? ModifiedAt { get; set; } // Nullable for optional modification tracking

    protected BonFullAggregateRoot () {}
    protected BonFullAggregateRoot(TId id)

    {
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    /// <summary>
    /// Marks the entity as deleted and sets the DeletedAt.
    /// </summary>
    public void SoftDelete()
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Restores a soft-deleted entity by unsetting IsDeleted and DeletedAt.
    /// </summary>
    public void Restore()
    {
        if (IsDeleted)
        {
            IsDeleted = false;
            DeletedAt = null;
        }
    }

    /// <summary>
    /// Updates the ModifiedAt, typically called whenever the entity is modified.
    /// </summary>
    public void UpdateModifiedDate()
    {
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the CreatedAt only if it has not been set previously.
    /// </summary>
    protected void SetCreationDate(DateTime creationDate)
    {
        if (CreatedAt == default)
        {
            CreatedAt = creationDate;
        }
    }
}