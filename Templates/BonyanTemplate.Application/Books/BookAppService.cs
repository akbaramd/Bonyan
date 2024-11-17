using Bonyan.Layer.Application.Abstractions;
using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Repository.Abstractions;
using BonyanTemplate.Application.Books.Dtos;
using BonyanTemplate.Domain.Books;

namespace BonyanTemplate.Application.Books;

public class BookAppService : CrudAppService<Book,BookId,BookDto>,IBookAppService
{
    public BookAppService(IBonRepository<Book, BookId> repository) : base(repository)
    {
    }
}