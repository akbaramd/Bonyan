using Bonyan.Layer.Domain;
using BonyanTemplate.Domain.Authors;

namespace BonyanTemplate.Infrastructure.Data.Repositories;

/// <summary>
/// Entity Framework Core implementation of <see cref="IAuthorsBonRepository"/>.
/// </summary>
public class EfAuthorBonRepository : EfCoreBonRepository<Author, AuthorId, BonyanTemplateDbContext>, IAuthorsBonRepository
{
}
