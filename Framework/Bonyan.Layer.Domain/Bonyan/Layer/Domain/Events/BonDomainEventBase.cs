namespace Bonyan.Layer.Domain.Events;

public abstract class BonDomainEventBase : IDomainEvent
{
  /// <summary>
  ///     Initializes a new instance of the <see cref="BonDomainEventBase" /> class.
  /// </summary>
  protected BonDomainEventBase()
    {
        DateOccurred = DateTime.UtcNow;
    }

  /// <summary>
  ///     Gets or sets the date and time when the event occurred.
  /// </summary>
  public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}

public interface IDomainEvent
{
}