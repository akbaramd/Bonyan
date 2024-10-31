using Bonyan.Layer.Domain;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;

namespace BonyanTemplate.Infrastructure.Data.Repositories;

public class EfBookRepository : EfCoreRepository<Books,BookId,BonyanTemplateBookManagementDbContext>, IBooksRepository
{
  public EfBookRepository(BonyanTemplateBookManagementDbContext bookManagementDbContext) : base(bookManagementDbContext)
  {
  }
}
