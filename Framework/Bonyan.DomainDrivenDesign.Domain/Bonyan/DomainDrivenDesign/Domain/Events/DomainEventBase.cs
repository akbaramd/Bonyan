namespace Bonyan.DomainDrivenDesign.Domain.Events;

public abstract class DomainEventBase : IDomainEvent
{
  /// <summary>
  /// Gets or sets the date and time when the event occurred.
  /// </summary>
  public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;

  /// <summary>
  /// Initializes a new instance of the <see cref="DomainEventBase"/> class.
  /// </summary>
  protected DomainEventBase()
  {
    DateOccurred = DateTime.UtcNow;
  }
}

public interface IDomainEvent
{
}