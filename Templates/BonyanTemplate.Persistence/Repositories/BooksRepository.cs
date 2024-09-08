using Bonyan.DDD.Domain;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;

namespace BonyanTemplate.Persistence;

public class  BooksRepository : EfCoreRepository<Books,Guid,AppDbContext>, IBooksRepository
{
  public BooksRepository(AppDbContext dbContext) : base(dbContext)
  {
  }
}
