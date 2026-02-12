using Bonyan.Layer.Domain.Aggregate;
using Bonyan.Layer.Domain.ValueObjects;
using BonyanTemplate.Domain.Authors;

namespace BonyanTemplate.Domain.Books;

/// <summary>
/// Book aggregate root.
/// </summary>
public class Book : BonAggregateRoot<BookId>
{
    /// <summary>
    /// Book title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Current availability status.
    /// </summary>
    public BookStatus Status { get; set; } = BookStatus.Available;

    /// <summary>
    /// Related author (navigation property).
    /// </summary>
    public Author Author { get; set; } = null!;

    /// <summary>
    /// Foreign key to the author.
    /// </summary>
    public AuthorId AuthorId { get; set; } = null!;
}

/// <summary>
/// Strongly-typed identifier for the <see cref="Book"/> aggregate.
/// </summary>
public class BookId : BonBusinessId<BookId>
{
}