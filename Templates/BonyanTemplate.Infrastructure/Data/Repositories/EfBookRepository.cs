using Bonyan.DomainDrivenDesign.Domain;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;

namespace BonyanTemplate.Infrastructure.Data.Repositories;

public class EfBookRepository : EfCoreRepository<Books,BookId,BonyanTemplateBookDbContext>, IBooksRepository
{
  public EfBookRepository(BonyanTemplateBookDbContext bookDbContext,IServiceProvider serviceProvider) : base(bookDbContext,serviceProvider)
  {
  }
}
