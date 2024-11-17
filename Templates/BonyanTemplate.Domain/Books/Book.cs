using Bonyan.Layer.Domain.Aggregate;
using Bonyan.Layer.Domain.ValueObjects;
using BonyanTemplate.Domain.Authors;

namespace BonyanTemplate.Domain.Books;

public class Book : BonAggregateRoot<BookId>
{
    public string Title { get; set; } = string.Empty;
    public BookStatus Status { get; set; } = BookStatus.Available;

    public Authors.Authors  Author { get; set; }
    public AuthorId  AuthorId { get; set; }
}

public class BookId : BonBusinessId<BookId>
{
}