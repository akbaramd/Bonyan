using Bonyan.Layer.Domain.Repository;
using Bonyan.Layer.Domain.Repository.Abstractions;
using BonyanTemplate.Domain.Entities;

namespace BonyanTemplate.Domain.Repositories;

public interface IBooksBonRepository : IBonRepository<Books, BookId>
{
}