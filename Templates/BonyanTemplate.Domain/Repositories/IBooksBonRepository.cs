using Bonyan.Layer.Domain.Abstractions;
using BonyanTemplate.Domain.Entities;

namespace BonyanTemplate.Domain.Repositories;

public interface IBooksBonRepository : IBonRepository<Books, BookId>
{
}