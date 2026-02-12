using Bonyan.Layer.Domain.Aggregate;
using Bonyan.Layer.Domain.ValueObjects;
using BonyanTemplate.Domain.Books.DomainEvents;

namespace BonyanTemplate.Domain.Authors;

/// <summary>
/// Author aggregate root. Use singular naming per enterprise/DDD conventions.
/// </summary>
public class Author : BonAggregateRoot<AuthorId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Author"/> class.
    /// </summary>
    public Author()
    {
        AddDomainEvent(new BookCreated());
    }

    /// <summary>
    /// Display name or title of the author.
    /// </summary>
    public string Title { get; set; } = string.Empty;
}

/// <summary>
/// Strongly-typed identifier for the <see cref="Author"/> aggregate.
/// </summary>
public class AuthorId : BonBusinessId<AuthorId>
{
}