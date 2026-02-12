using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using BonyanTemplate.Application.Books.Dtos;
using BonyanTemplate.Domain.Books;

namespace BonyanTemplate.Application.Books;

/// <summary>
/// Application service for <see cref="Domain.Books.Book"/> CRUD operations.
/// </summary>
public interface IBookAppService : IBonCrudAppService<BookId, BonPaginateDto, BookDto>
{
}