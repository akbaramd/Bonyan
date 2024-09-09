using System.Linq.Expressions;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.Persistence.EntityFrameworkCore;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Enums;

namespace BonyanTemplate.Persistence.Seeds;

public class BookSeed : ISeeder
{
  private IRepository<Books, Guid> _repository;

  public BookSeed(IRepository<Books, Guid> repository)
  {
    _repository = repository;
  }

  public async Task SeedAsync(CancellationToken cancellationToken = default)
  {
    var bookToSeed = new Books
    {
      Id = Guid.Parse("9f44544d-1d04-401f-a365-ce699240503c"),
      Title = "test",
      Status = BookStatus.Available
    };

    // Check if a book with the same Id already exists in the repository
    var existingBook = await _repository.GetByIdAsync(bookToSeed.Id);

    if (existingBook == null)
    {
      // If the book does not exist, insert it into the repository
      await _repository.AddAsync(bookToSeed);
    }
    
  }
}
