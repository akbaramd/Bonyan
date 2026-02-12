using Bonyan.Layer.Application.Dto;
using BonyanTemplate.Domain.Authors;
using BonyanTemplate.Domain.Books;

namespace BonyanTemplate.Application.Books.Dtos;

/// <summary>
/// Data transfer object for <see cref="Domain.Books.Book"/>.
/// </summary>
public class BookDto : IBonAggregateRootDto<BookId>
{
    /// <summary>Aggregate id.</summary>
    public BookId Id { get; set; } = null!;

    /// <summary>Book title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Author id.</summary>
    public AuthorId AuthorId { get; set; } = null!;
}

