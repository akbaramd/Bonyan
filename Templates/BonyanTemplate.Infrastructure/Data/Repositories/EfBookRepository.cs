using Bonyan.Layer.Domain;
using BonyanTemplate.Domain.Books;

namespace BonyanTemplate.Infrastructure.Data.Repositories;

public class EfBookRepository : EfCoreBonRepository<Book,BookId,TemplateBookManagementBonDbContext>, IBooksRepository
{
 
}
