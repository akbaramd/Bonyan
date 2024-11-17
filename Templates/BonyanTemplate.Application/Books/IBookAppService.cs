using Bonyan.Layer.Application.Abstractions;
using Bonyan.Layer.Application.Dto;
using BonyanTemplate.Application.Books.Dtos;
using BonyanTemplate.Domain.Books;

namespace BonyanTemplate.Application.Books;

public interface IBookAppService : ICrudAppService<BookId,BonPaginateDto,BookDto>
{
    
}