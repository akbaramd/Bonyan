using Bonyan.Layer.Domain;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;

namespace BonyanTemplate.Infrastructure.Data.Repositories;

public class EfAuthorRepository : EfCoreRepository<Authors,AuthorId,BonyanTemplateBookManagementDbContext>, IAuthorsRepository
{
  public EfAuthorRepository(BonyanTemplateBookManagementDbContext bookManagementDbContext) : base(bookManagementDbContext)
  {
  }
}
