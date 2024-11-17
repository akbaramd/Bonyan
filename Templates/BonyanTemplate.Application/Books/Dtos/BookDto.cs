using Bonyan.Layer.Application.Dto;
using BonyanTemplate.Domain.Authors;
using BonyanTemplate.Domain.Books;

namespace BonyanTemplate.Application.Books.Dtos;

public class BookDto : IBonAggregateRootDto<BookId>
{
    public BookId Id { get; set; }

    public string Title { get; set; }
    public AuthorId AuthorId { get; set; }
}

