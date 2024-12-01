using Bonyan.Layer.Domain;
using BonyanTemplate.Domain.Authors;

namespace BonyanTemplate.Infrastructure.Data.Repositories;

public class EfAuthorBonRepository : EfCoreBonRepository<Authors,AuthorId,BonyanTemplateDbContext>, IAuthorsBonRepository
{
 
}
