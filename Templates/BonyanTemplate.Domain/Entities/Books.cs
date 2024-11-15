using Bonyan.Layer.Domain.Aggregate;
using Bonyan.Layer.Domain.ValueObjects;
using BonyanTemplate.Domain.Enums;

namespace BonyanTemplate.Domain.Entities;

public class Books : BonAggregateRoot<BookId>
{
    public string Title { get; set; } = string.Empty;
    public BookStatus Status { get; set; } = BookStatus.Available;

    public Authors  Author { get; set; }
    public AuthorId  AuthorId { get; set; }
}

public class BookId : BonBusinessId<BookId>
{
}