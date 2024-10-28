using Bonyan.Layer.Domain.Aggregates;
using Bonyan.Layer.Domain.ValueObjects;
using BonyanTemplate.Domain.Enums;

namespace BonyanTemplate.Domain.Entities;

public class Books : TenantAggregateRoot<BookId>
{
  public string Title { get; set; } = string.Empty;
  public BookStatus Status { get; set; } = BookStatus.Available;
}

public class BookId : BusinessId<BookId>
{
  
}
