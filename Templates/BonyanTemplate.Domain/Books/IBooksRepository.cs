using Bonyan.Layer.Domain.Repository.Abstractions;

namespace BonyanTemplate.Domain.Books;

public interface IBooksRepository : IBonRepository<Book, BookId>
{
}