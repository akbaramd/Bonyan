using Bonyan.AspNetCore.Persistence;
using Bonyan.DDD.Domain;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;

namespace BonyanTemplate.Infrastructure.Data.Repositories;

public class MemoryBookRepository : InMemoryRepository<Books,Guid>, IBooksRepository
{
  public MemoryBookRepository(ITenantAccessor accessor) : base(accessor)
  {
  }
}
