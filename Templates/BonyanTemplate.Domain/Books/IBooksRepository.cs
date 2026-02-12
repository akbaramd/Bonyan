using Bonyan.Layer.Domain.Repository.Abstractions;

namespace BonyanTemplate.Domain.Books;

/// <summary>
/// Repository for <see cref="Book"/> aggregate root.
/// </summary>
public interface IBooksRepository : IBonRepository<Book, BookId>
{
}