namespace Bonyan.DomainDrivenDesign.Domain.Entities;

/// <summary>
/// Abstract factory class for creating and configuring instances of entities.
/// </summary>
/// <typeparam name="TEntity">The type of entity this factory creates.</typeparam>
public abstract class EntityFactory<TEntity>
{
  /// <summary>
  /// Initializes a new instance of the entity.
  /// </summary>
  /// <returns>A newly created entity of type <typeparamref name="TEntity"/>.</returns>
  public abstract TEntity Initialize();

  /// <summary>
  /// Validates the entity after creation. Can be overridden in derived classes to provide custom validation logic.
  /// </summary>
  /// <param name="entity">The entity to validate.</param>
  protected virtual void Validate(TEntity entity)
  {
    // Default implementation is a no-op. Override in derived classes for custom validation.
  }

  /// <summary>
  /// Configures the entity after validation. Can be overridden in derived classes to provide custom configuration logic.
  /// </summary>
  /// <param name="entity">The entity to configure.</param>
  protected virtual void Configure(TEntity entity)
  {
    // Default implementation is a no-op. Override in derived classes for custom configuration.
  }

  /// <summary>
  /// Creates, validates, and configures a new instance of the entity.
  /// </summary>
  /// <returns>A fully initialized, validated, and configured entity of type <typeparamref name="TEntity"/>.</returns>
  public TEntity Create()
  {
    var entity = Initialize();

    Validate(entity);
    Configure(entity);

    return entity;
  }
}
