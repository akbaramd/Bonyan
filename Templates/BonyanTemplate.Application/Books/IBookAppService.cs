using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using BonyanTemplate.Application.Books.Dtos;
using BonyanTemplate.Domain.Books;

namespace BonyanTemplate.Application.Books;

public interface IBookAppService : IBonCrudAppService<BookId,BonPaginateDto,BookDto>
{
    
}