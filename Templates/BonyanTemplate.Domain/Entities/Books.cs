using Bonyan.Layer.Domain.Aggregates;
using Bonyan.Layer.Domain.ValueObjects;
using Bonyan.MultiTenant;
using BonyanTemplate.Domain.Enums;

namespace BonyanTemplate.Domain.Entities;

public class Books : AggregateRoot<BookId>
{
    public string Title { get; set; } = string.Empty;
    public BookStatus Status { get; set; } = BookStatus.Available;

    public Authors  Author { get; set; }
    public AuthorId  AuthorId { get; set; }
}

public class BookId : BusinessId<BookId>
{
}