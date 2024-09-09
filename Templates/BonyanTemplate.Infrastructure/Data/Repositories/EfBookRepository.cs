using Bonyan.DDD.Domain;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;

namespace BonyanTemplate.Infrastructure.Data.Repositories;

public class EfBookRepository : EfCoreRepository<Books,Guid,BonyanTemplateDbContext>, IBooksRepository
{
  public EfBookRepository(BonyanTemplateDbContext dbContext) : base(dbContext)
  {
  }
}
