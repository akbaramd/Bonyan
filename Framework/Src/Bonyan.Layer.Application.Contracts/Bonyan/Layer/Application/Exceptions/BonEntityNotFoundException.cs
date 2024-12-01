using System.Net;

namespace Bonyan.Layer.Application.Exceptions;

/// <summary>
/// This exception is thrown when an entity is expected to be found but is not found.
/// </summary>
public class BonEntityNotFoundException : BonApplicationException
{
    /// <summary>
    /// Type of the entity.
    /// </summary>
    public Type? EntityType { get; set; }

    /// <summary>
    /// Id of the Entity.
    /// </summary>
    public object? Id { get; set; }

    /// <summary>
    /// Creates a new <see cref="BonEntityNotFoundException"/> object.
    /// </summary>
    public BonEntityNotFoundException()
        : base(HttpStatusCode.NotFound, "The specified entity was not found.")
    {
    }

    /// <summary>
    /// Creates a new <see cref="BonEntityNotFoundException"/> object.
    /// </summary>
    public BonEntityNotFoundException(Type entityType)
        : this(entityType, null, null)
    {
    }

    /// <summary>
    /// Creates a new <see cref="BonEntityNotFoundException"/> object.
    /// </summary>
    public BonEntityNotFoundException(Type entityType, object? id)
        : this(entityType, id, null)
    {
    }

    /// <summary>
    /// Creates a new <see cref="BonEntityNotFoundException"/> object.
    /// </summary>
    public BonEntityNotFoundException(Type entityType, object? id, Exception? innerException)
        : base(
            HttpStatusCode.NotFound,
            id == null
                ? $"Entity of type {entityType.FullName} was not found."
                : $"Entity of type {entityType.FullName} with id {id} was not found.",
            errorCode: "EntityNotFound",
            parameters: new { EntityType = entityType.FullName, Id = id })
    {
        EntityType = entityType;
        Id = id;
    }

    /// <summary>
    /// Creates a new <see cref="BonEntityNotFoundException"/> object with a custom message.
    /// </summary>
    /// <param name="message">Exception message</param>
    public BonEntityNotFoundException(string message)
        : base(HttpStatusCode.NotFound, message, errorCode: "EntityNotFound")
    {
    }

    /// <summary>
    /// Creates a new <see cref="BonEntityNotFoundException"/> object with a custom message and inner exception.
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public BonEntityNotFoundException(string message, Exception innerException)
        : base(HttpStatusCode.NotFound, message, errorCode: "EntityNotFound", parameters: null)
    {
    }
}
