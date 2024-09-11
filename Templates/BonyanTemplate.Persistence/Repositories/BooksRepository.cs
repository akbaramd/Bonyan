using Bonyan.DDD.Domain;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;

namespace BonyanTemplate.Persistence.Repositories;

public class  BooksRepository : EfCoreRepository<Books,Guid,AppDbContext>, IBooksRepository
{
  public BooksRepository(AppDbContext dbContext, IServiceProvider serviceProvider) : base(dbContext, serviceProvider)
  {
  }
}
