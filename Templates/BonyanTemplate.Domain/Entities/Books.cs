using Bonyan.DDD.Domain.Aggregates;
using BonyanTemplate.Domain.Enums;

namespace BonyanTemplate.Domain.Entities;

public class Books : AggregateRoot<Guid>
{
  public string Title { get; set; } = string.Empty;
  public BookStatus Status { get; set; } = BookStatus.Available;
}
