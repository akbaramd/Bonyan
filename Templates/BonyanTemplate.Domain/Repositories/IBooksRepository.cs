using Bonyan.Layer.Domain.Abstractions;
using BonyanTemplate.Domain.Entities;

namespace BonyanTemplate.Domain.Repositories;

public interface IBooksRepository : IRepository<Books, BookId>
{
}