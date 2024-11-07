using Bonyan.Layer.Domain;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;

namespace BonyanTemplate.Infrastructure.Data.Repositories;

public class EfBookBonRepository : EfCoreBonRepository<Books,BookId,BonTemplateBookManagementDbContext>, IBooksBonRepository
{
  public EfBookBonRepository(BonTemplateBookManagementDbContext bookManagementDbContext) : base(bookManagementDbContext)
  {
  }
}
