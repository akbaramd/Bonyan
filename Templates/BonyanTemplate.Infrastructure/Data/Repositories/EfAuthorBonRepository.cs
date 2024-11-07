using Bonyan.Layer.Domain;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;

namespace BonyanTemplate.Infrastructure.Data.Repositories;

public class EfAuthorBonRepository : EfCoreBonRepository<Authors,AuthorId,BonTemplateBookManagementDbContext>, IAuthorsBonRepository
{
  public EfAuthorBonRepository(BonTemplateBookManagementDbContext bookManagementDbContext) : base(bookManagementDbContext)
  {
  }
}
