using Bonyan.AspNetCore.Persistence;
using Bonyan.DDD.Domain;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;

namespace BonyanTemplate.Infrastructure.Data.Repositories;

public class MemoryBookRepository : InMemoryRepository<Books,Guid>, IBooksRepository
{
  
}
